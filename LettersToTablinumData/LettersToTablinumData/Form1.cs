using System;
using System.IO;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LettersToTablinumData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private IMongoCollection<Initio> _initios;
        private IMongoCollection<Group> _groups;
        private IMongoCollection<Document> _documents;
        public string path = @"C:\Users\Ned Meloun\source\repos\TablinumDB\department\department\backup_release_output.sql";
        public string pathO = @"C:\Users\Ned Meloun\source\repos\TablinumDB\otdel\otdel\backup_release_output.sql";
        public string connectionString = "mongodb://localhost:27017";
        public MongoClient client;
        public IMongoDatabase database;
        public List<Initio> oldInitio;
        public List<Group> oldGroup;
        public List<DocumentGroup> oldLocation;
        public List<DocumentGroup> location;

        private void button1_Click(object sender, EventArgs e)
        {
            oldInitio = new List<Initio>();
            oldGroup = new List<Group>();
            oldLocation = new List<DocumentGroup>();
            location = new List<DocumentGroup>();
            AddInitio();
            AddGroup();
            ReadLocation();
            AddDocument();
            AddNumberOtdel();
        }

        public void AddInitio()
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("tablinum");
            _initios = database.GetCollection<Initio>("Initio");
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string line;
                bool flagInitio = false;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "/*!40000 ALTER TABLE `templateinitio` DISABLE KEYS */;")
                    {
                        flagInitio = true;
                    }
                    if (line == "/*!40000 ALTER TABLE `templateinitio` ENABLE KEYS */;")
                    {
                        flagInitio = false;
                    }
                    if (flagInitio && line != "" && line.Contains("("))
                    {
                        line = line.TrimStart('(').TrimEnd(',').TrimEnd(')');
                        string[] splLine = line.Split(',');
                        Initio initio = new Initio();
                        initio.Executor = splLine[1].TrimStart('\'').TrimEnd('\'').Replace("\\", "");
                        _initios.InsertOne(initio);
                        Initio old = new Initio();
                        old.Id = splLine[0];
                        old.Executor = splLine[1].TrimStart('\'').TrimEnd('\'').Replace("\\", "");
                        oldInitio.Add(old);
                    }
                }
            }
        }

        public void AddGroup()
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("tablinum");
            _groups = database.GetCollection<Group>("Groups");
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string line;
                bool flagGroup = false;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "/*!40000 ALTER TABLE `templatelocationname` DISABLE KEYS */;")
                    {
                        flagGroup = true;
                    }
                    if (line == "/*!40000 ALTER TABLE `templatelocationname` ENABLE KEYS */;")
                    {
                        flagGroup = false;
                    }
                    if (flagGroup && line != "" && line.Contains("("))
                    {
                        line = line.TrimStart('(').TrimEnd(',').TrimEnd(')');
                        string[] splLine = line.Split(',');
                        Group group = new Group();
                        group.GroupName = splLine[1].TrimStart('\'').TrimEnd('\'').Replace("\\", "");
                        _groups.InsertOne(group);
                        Group old = new Group();
                        old.Id = splLine[0];
                        old.GroupName = splLine[1].TrimStart('\'').TrimEnd('\'').Replace("\\", "");
                        oldGroup.Add(old);
                    }
                }
            }
        }

        public void ReadLocation()
        {
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string line;
                bool flagLocation = false;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line == "/*!40000 ALTER TABLE `locations` DISABLE KEYS */;")
                    {
                        flagLocation = true;
                    }
                    if (line == "/*!40000 ALTER TABLE `locations` ENABLE KEYS */;")
                    {
                        flagLocation = false;
                    }
                    if (flagLocation && line != "" && line.Contains("("))
                    {
                        line = line.TrimStart('(').TrimEnd(',').TrimEnd(')');
                        string[] splLine = line.Split(',');
                        DocumentGroup docgroup = new DocumentGroup();
                        docgroup.GroupId = splLine[2];
                        docgroup.NumberGroup = splLine[1];
                        docgroup.NumberGroupDate = DateTime.ParseExact(splLine[3].TrimStart('\'').TrimEnd('\''), "yyyy-MM-dd HH:mm:ss", 
                                                                       System.Globalization.CultureInfo.InvariantCulture);
                        oldLocation.Add(docgroup);
                    }
                }
            }
        }

        public void AddDocument()
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("tablinum");
            _documents = database.GetCollection<Document>("Documents");
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string line;
                bool flagDocument = false;
                while ((line = sr.ReadLine()) != null)
                {
                    List<Initio> init = _initios.Find(initio => true).ToList();
                    List<Group> grp = _groups.Find(group => true).ToList();
                    if (line == "/*!40000 ALTER TABLE `documents` DISABLE KEYS */;")
                    {
                        flagDocument = true;
                    }
                    if (line == "/*!40000 ALTER TABLE `documents` ENABLE KEYS */;")
                    {
                        flagDocument = false;
                    }
                    if (flagDocument && line != "" && line.Contains("("))
                    {
                        line = line.TrimStart('(').TrimEnd(',').TrimEnd(')');
                        string[] splLine = line.Split(',');
                        DocumentGroup dg = new DocumentGroup();
                        dg.GroupId = // secretary group;
                        dg.NumberGroup = "";
                        dg.NumberGroupDate = new DateTime();
                        dg.Location = false;
                        location.Add(dg);
                        for (int i = 0; i < oldLocation.Count; i++)
                        {
                            if (oldLocation[i].NumberGroup == splLine[0])
                            {
                                dg.GroupId = grp.Find(item => item.GroupName == oldGroup.Find(it => 
                                                                                it.Id == oldLocation[i].GroupId).GroupName).Id;
                                dg.NumberGroup = "";
                                dg.NumberGroupDate = oldLocation[i].NumberGroupDate;
                                dg.Location = false;
                                location.Add(dg);
                            }
                        }
                        try
                        {
                            Document document = new Document();
                            document.DocumentNumber = (splLine[1] != "NULL") ? splLine[1].TrimStart('\'').TrimEnd('\'') : "";
                            document.NumberDate = (splLine[2] != "NULL") ? DateTime.ParseExact(splLine[2].TrimStart('\'').TrimEnd('\''),
                                                                           "yyyy-MM-dd HH:mm:ss",
                                                                           System.Globalization.CultureInfo.InvariantCulture) :
                                                                           DateTime.ParseExact("0001-01-01 00:00:00",
                                                                           "yyyy-MM-dd HH:mm:ss",
                                                                           System.Globalization.CultureInfo.InvariantCulture);
                            document.NumberCenter = (splLine[3] != "NULL") ? splLine[3].TrimStart('\'').TrimEnd('\'') : "";
                            document.NumberCenterDate = (splLine[4] != "NULL") ? DateTime.ParseExact(splLine[4].TrimStart('\'').TrimEnd('\''),
                                                                                 "yyyy-MM-dd HH:mm:ss",
                                                                                 System.Globalization.CultureInfo.InvariantCulture) :
                                                                                 DateTime.ParseExact("0001-01-01 00:00:00",
                                                                                 "yyyy-MM-dd HH:mm:ss",
                                                                                 System.Globalization.CultureInfo.InvariantCulture);
                            document.NumberDepartment = (splLine[5] != "NULL") ? splLine[5].TrimStart('\'').TrimEnd('\'') : "";
                            document.NumberDepartmentDate = (splLine[6] != "NULL") ? DateTime.ParseExact(splLine[6].TrimStart('\'').TrimEnd('\''),
                                                                                     "yyyy-MM-dd HH:mm:ss",
                                                                                     System.Globalization.CultureInfo.InvariantCulture) :
                                                                                     DateTime.ParseExact("0001-01-01 00:00:00",
                                                                                     "yyyy-MM-dd HH:mm:ss",
                                                                                     System.Globalization.CultureInfo.InvariantCulture);
                            document.GroupInfo = location;
                            document.InitioId = init.Find(item => item.Executor == oldInitio.Find(it => it.Id == splLine[7]).Executor).Id;
                            document.UserId = "60e03b07e18ce904c8e04ee4";
                            document.ExecutionDate = (splLine[15] != "NULL" && 
                                                      splLine[15] != "'1753-01-01 00:00:00'") ? 
                                                                               DateTime.ParseExact(splLine[15].TrimStart('\'').TrimEnd('\''),
                                                                               "yyyy-MM-dd HH:mm:ss",
                                                                               System.Globalization.CultureInfo.InvariantCulture) :
                                                                               DateTime.ParseExact("0001-01-01 00:00:00",
                                                                               "yyyy-MM-dd HH:mm:ss",
                                                                               System.Globalization.CultureInfo.InvariantCulture);
                            document.Status = (splLine[16] == "1");
                            document.View = (splLine[8] != "NULL") ? splLine[8].TrimStart('\'').TrimEnd('\'') : "";
                            document.Speed = (splLine[9] != "NULL") ? splLine[9].TrimStart('\'').TrimEnd('\'') : "";
                            document.Control = (splLine[10] == "1");
                            document.Comment = ((splLine[14] != "NULL") ? splLine[14].TrimStart('\'').TrimEnd('\'') : "") + " " +
                                               ((splLine[11] != "NULL") ? splLine[11].TrimStart('\'').TrimEnd('\'') : "");
                            document.Created = (splLine[12] != "NULL") ? DateTime.ParseExact(splLine[12].TrimStart('\'').TrimEnd('\''),
                                                                         "yyyy-MM-dd HH:mm:ss",
                                                                         System.Globalization.CultureInfo.InvariantCulture) :
                                                                         DateTime.ParseExact("0001-01-01 00:00:00",
                                                                         "yyyy-MM-dd HH:mm:ss",
                                                                         System.Globalization.CultureInfo.InvariantCulture);
                            document.Updated = (splLine[13] != "NULL") ? DateTime.ParseExact(splLine[13].TrimStart('\'').TrimEnd('\''),
                                                                         "yyyy-MM-dd HH:mm:ss",
                                                                         System.Globalization.CultureInfo.InvariantCulture) :
                                                                         DateTime.ParseExact("0001-01-01 00:00:00",
                                                                         "yyyy-MM-dd HH:mm:ss",
                                                                         System.Globalization.CultureInfo.InvariantCulture);
                            _documents.InsertOne(document);
                        }
                        catch (Exception e) {
                            Console.WriteLine(e);
                        }
                        location.Clear();
                    }
                }
            }
        }

        public void AddNumberOtdel()
        {
            client = new MongoClient(connectionString);
            database = client.GetDatabase("tablinum");
            _documents = database.GetCollection<Document>("Documents");
            using (StreamReader sr = new StreamReader(pathO, System.Text.Encoding.UTF8))
            {
                string line;
                bool flagDocument = false;
                while ((line = sr.ReadLine()) != null)
                {
                    List<Initio> init = _initios.Find(initio => true).ToList();
                    List<Group> grp = _groups.Find(group => true).ToList();
                    if (line == "/*!40000 ALTER TABLE `documents` DISABLE KEYS */;")
                    {
                        flagDocument = true;
                    }
                    if (line == "/*!40000 ALTER TABLE `documents` ENABLE KEYS */;")
                    {
                        flagDocument = false;
                    }
                    if (flagDocument && line != "" && line.Contains("("))
                    {
                        // 12345
                        line = line.TrimStart('(').TrimEnd(',').TrimEnd(')');
                        string[] splLine = line.Split(',');
                        for (int i = 0; i < oldLocation.Count; i++)
                        {
                            if (oldLocation[i].NumberGroup == splLine[0])
                            {
                                DocumentGroup dg = new DocumentGroup();
                                dg.GroupId = grp.Find(item => item.GroupName == oldGroup.Find(it =>
                                                                                it.Id == oldLocation[i].GroupId).GroupName).Id;
                                dg.NumberGroup = "";
                                dg.NumberGroupDate = oldLocation[i].NumberGroupDate;
                                dg.Location = false;
                                location.Add(dg);
                            }
                        }
                        try
                        {
                            foreach (var loc in locationsID)
                            {
                                var filter = Builders<Document>.Filter.Eq(x => x.Id, id) & Builders<Document>.Filter.ElemMatch(x => x.GroupInfo,
                                                                                       Builders<DocumentGroup>.Filter.Eq(x => x.GroupId, loc));
                                var update = Builders<Document>.Update.Set(x => x.GroupInfo[-1].Location, false);
                                _documents.UpdateOneAsync(filter, update);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        location.Clear();
                    }
                }
            }
        }
    }

    public class Initio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Initio")]
        [JsonProperty("Initio")]
        public string Executor { get; set; }
    }

    public class Group
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Dept")]
        [JsonProperty("Dept")]
        public string GroupName { get; set; }
    }

    public class DocumentGroup
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Group")]
        [JsonProperty("Group")]
        public string GroupId { get; set; }
        public string NumberGroup { get; set; }
        public DateTime NumberGroupDate { get; set; }
        public bool Location { get; set; }
    }

    public class Document
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Number")]
        [JsonProperty("Number")]
        public string DocumentNumber { get; set; }

        public DateTime NumberDate { get; set; }

        public string NumberCenter { get; set; }

        public DateTime NumberCenterDate { get; set; }
        public string NumberDepartment { get; set; }
        public DateTime NumberDepartmentDate { get; set; }
        public List<DocumentGroup> GroupInfo { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("From")]
        [JsonProperty("From")]
        public string InitioId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Executor")]
        [JsonProperty("Executor")]
        public string UserId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public bool Status { get; set; }
        public string View { get; set; }
        public string Speed { get; set; }
        public bool Control { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
