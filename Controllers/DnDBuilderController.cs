using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using Mono.Data.Sqlite;
using Newtonsoft.Json.Linq;

namespace DnD_Builder.Controllers
{
    public class DnDBuilderController : ApiController
    {
        public DnDBuilderController()
        {
            try
            {
                //Create the database
                createDatabase();
                //Create the tables
                createTables();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Already created");
            }
        }


        /* Function: SearchCharacter
         * Type: Get
         * Parameters: name (String)
         * Return: Dictionary<string, object>
         * Assertion:  function returns character information given the name
         */
        [HttpGet]
        [Route("api/searchCharacter/{name}")]
        public Dictionary<string, object> SearchCharacter(string name)
        {
            Dictionary<string, object> res = null;
            try
            {
                //Validation
                checkEmpty(name);

                //Search the character in the database
                res = searchCharacter(name);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.NotFound, "Error occurred : " + ex.Message));
            }
            return res;
        }

        /* Function: SearchAllCharacters
         * Type: Post
         * Parameters: none
         * Return: Dictionary<string, object>
         * Assertion: function returns all characters information
         */
        [HttpGet]
        [Route("api/searchAllCharacters")]
        public Dictionary<string, object> SearchAllCharacters()
        {
            Dictionary<string, object> res = null;
            try
            {
                //Search all characters in the database
                res = searchAllCharacters();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred : " + ex.Message));
            }
            return res;
        }

        /* Function: AddCharacter
        * Type: Post
        * Parameters: none
        * Return: int
        * Assertion: function adds a new character to the database
        */
        [HttpPost]
        [Route("api/addCharacter")]
        public int AddCharacter([FromBody]Dictionary<string, object> req)
        {
            try
            {
                //Variables
                string name = Convert.ToString(req["name"]);
                int age = Convert.ToInt32(req["age"]);
                string gender = Convert.ToString(req["gender"]);
                string biography = Convert.ToString(req["biography"]);
                int level = Convert.ToInt32(req["level"]);
                string race = Convert.ToString(req["race"]);
                string cclass = Convert.ToString(req["class"]);
                int constitution = Convert.ToInt32(req["constitution"]);
                int dexterity = Convert.ToInt32(req["dexterity"]);
                int strength = Convert.ToInt32(req["strength"]);
                int charisma = Convert.ToInt32(req["charisma"]);
                int inteligence = Convert.ToInt32(req["inteligence"]);
                int wisdom = Convert.ToInt32(req["wisdom"]);

                //Validation
                checkEmpty(name);
                checkAge(age);
                checkBio(biography);
                checkLevel(level);
                checkRC("http://www.dnd5eapi.co/api/races", race);
                checkRC("http://www.dnd5eapi.co/api/classes", cclass);
                checkAbilities(constitution, dexterity, strength, charisma, inteligence, wisdom);

                //Add character to the database
                addCharacter(name, age, gender, biography, level, race, cclass, constitution, dexterity, strength, charisma, inteligence, wisdom);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred : " + ex.Message));
            }
            return 0;
        }

        /* Function: DeleteCharacter
        * Type: Post
        * Parameters: none
        * Return: int
        * Assertion: function deletes a character from the database
        */
        [HttpPost]
        [Route("api/deleteCharacter")]
        public int DeleteCharacter([FromBody]Dictionary<string, object> req)
        {
            try
            {
                //Variables
                string name = Convert.ToString(req["name"]);

                //Validation
                checkEmpty(name);

                //Delete character from the database
                deleteCharacter(name);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred : " + ex.Message));
            }
            return 0;
        }

        /* Function: EditCharacter
        * Type: Post
        * Parameters: none
        * Return: int
        * Assertion: function edits a character in the database
        */
        [HttpPost]
        [Route("api/editCharacter")]
        public int EditCharacter([FromBody]Dictionary<string, object> req)
        {
            try
            {
                //Variables
                string name = Convert.ToString(req["name"]);
                int age = Convert.ToInt32(req["age"]);
                string gender = Convert.ToString(req["gender"]);
                string biography = Convert.ToString(req["biography"]);
                int level = Convert.ToInt32(req["level"]);
                string race = Convert.ToString(req["race"]);
                string cclass = Convert.ToString(req["class"]);
                int constitution = Convert.ToInt32(req["constitution"]);
                int dexterity = Convert.ToInt32(req["dexterity"]);
                int strength = Convert.ToInt32(req["strength"]);
                int charisma = Convert.ToInt32(req["charisma"]);
                int inteligence = Convert.ToInt32(req["inteligence"]);
                int wisdom = Convert.ToInt32(req["wisdom"]);

                //Validation
                checkEmpty(name);
                checkAge(age);
                checkBio(biography);
                checkLevel(level);
                checkRC("http://www.dnd5eapi.co/api/races", race);
                checkRC("http://www.dnd5eapi.co/api/classes", cclass);
                checkAbilities(constitution, dexterity, strength, charisma, inteligence, wisdom);

                //Edit the character in the database
                editCharacter(name, age, gender, biography, level, race, cclass, constitution, dexterity, strength, charisma, inteligence, wisdom);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error occurred : " + ex.Message));
            }
            return 0;
        }

        /* Function: dnd5eapi
        * Type: Post
        * Parameters: none
        * Return: Dictionary<string, object>
        * Assertion: function returns all characters information
        */
        [HttpGet]
        [Route("api/dnd5eapi/{race}/{cclass}")]
        public Dictionary<string, object> dnd5eapi(string race, string cclass)
        {
            Dictionary<string, object> res = null;
            try
            {
                //Variables
                string raceNum = getUrlIndex("http://www.dnd5eapi.co/api/races", race);
                string classNum = getUrlIndex("http://www.dnd5eapi.co/api/classes", cclass);

                //Create a dictionary and add the infomation to it
                res = new Dictionary<string, object>();
                res.Add("race_info", getRaceInfo(raceNum));
                res.Add("class_info", getClassInfo(classNum));
                return res;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error has occured, please try again later!"));
            }

        }

        [HttpGet]
        [Route("api/dnd5eapi/RCList")]
        public Dictionary<string, object> dnd5eapiList()
        {
            Dictionary<string, object> res = null;
            try
            {
                //Variables
                Dictionary<string, object> raceList = getUrlList("http://www.dnd5eapi.co/api/races");
                Dictionary<string, object> classList = getUrlList("http://www.dnd5eapi.co/api/classes");

                //Create a dictionary and add the infomation to it
                res = new Dictionary<string, object>();
                res.Add("race_list", raceList);
                res.Add("class_list", classList);
                return res;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(this.Request.CreateResponse<object>(HttpStatusCode.InternalServerError, "Error has occured, please try again later!"));
            }

        }

        private Dictionary<string, object> getUrlList(string url)
        {
            string res = null;
            Dictionary<string, object> answer = new Dictionary<string, object>();

            //Create new web request
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            //Response
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            //Get and read data
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            res = sanatiseData(res);
            response.Close();
            JObject job = JObject.Parse(res);
            JArray array = (JArray)job["results"];

            answer.Add("results", array);
            //If nothing found throw an error
            if (res == null)
            {
                throw new Exception("Sorry nothing found!");
            }
            return answer;
        }

        private string getUrlIndex(string url, string type)
        {
            string res = null;

            //Create new web request
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            //Response
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            //Get and read data
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            res = sanatiseData(res);
            response.Close();
            JObject job = JObject.Parse(res);

            //Loop through each result and see if it matches the selected type
            for (int i = 0; i < (int)job.SelectToken("count"); i++)
            {
                //If it does match the name then get the url and split it
                if ((string)job.SelectToken("results["+i+"].name") == type)
                {
                    //Gather the index of the type to use to gather futher information later
                    string url2 = (string)job.SelectToken("results[" + i + "].url");
                    string[] urlarray = url2.Split('/');
                    res = urlarray[urlarray.Length - 1];
                }
            }
            //If nothing found throw an error
            if (res == null)
            {
                throw new Exception("Sorry nothing found!");
            }
            return res;
        }

        private Dictionary<string, object> getRaceInfo(string raceNum)
        {
            Dictionary<string, object> answer = new Dictionary<string, object>();
            string res = null;
            string race_url = "http://www.dnd5eapi.co/api/races/" + raceNum;

            //Create new web request
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(race_url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            //Response
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            //Get and read data
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            res = sanatiseData(res);
            response.Close();
            //Parse the data
            JObject race_obj = JObject.Parse(res);
            JArray array = (JArray)race_obj["ability_bonuses"];

            //Gather to racial total, which later will be used client side
            int racial_total = 0;
            foreach (int ability in array)
            {
                racial_total += ability;
            }

            answer.Add("racial_total", racial_total);
            answer.Add("constitution_score", array.First);
            return answer;
        }

        private Dictionary<string, object> getClassInfo(string classNum)
        {
            Dictionary<string, object> answer = new Dictionary<string, object>();
            string res = null;
            string class_url = "http://www.dnd5eapi.co/api/classes/" + classNum;

            //Create new web request
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(class_url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "GET";

            //Response
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            //Get and read data
            Stream data = response.GetResponseStream();
            StreamReader reader = new StreamReader(data);
            res = reader.ReadToEnd();
            res = sanatiseData(res);
            response.Close();
            JObject class_obj = JObject.Parse(res);

            //See if the class is a spell caster or not
            string spellcaster = "No";
            if (class_obj.SelectToken("spellcasting") != null)
            {
                spellcaster = "Yes";
            }

            answer.Add("hit_die", class_obj.SelectToken("hit_die"));
            answer.Add("spellcaster", spellcaster);
            return answer;
        }

        void createDatabase()
        {
            try
            {
                //check is the database file already exists
                if (!File.Exists("MyDatabase.sqlite"))
                {

                    //create the database
                    SqliteConnection.CreateFile("MyDatabase.sqlite");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Database already exists! You good: " + e.Message);
            }
        }

        void createTables()
        {
            try
            {
                //connect to database
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=MyDatabase.sqlite;Version=3;"))
                {
                    m_dbConn.Open();
                    string sql = "CREATE TABLE character (name varchar(20) primary key, age int(3), gender varchar(20), biography varchar(500), level int(2), race varchar(100), class varchar(100), constitution int(2), dexterity int(2), strength int(2), charisma int(2), inteligence int(2), wisdom int(2))";
                    SqliteCommand command = new SqliteCommand(sql, m_dbConn); 
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Table already exists! You good: "+ e.Message);
            }

        }

        //Search all characters
        Dictionary<string, object> searchAllCharacters()
        {
            Dictionary<string, object> res = null;
            try
            {
                int ii = 1;
                //connect to database
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=MyDatabase.sqlite;Version=3;"))
                {
                    m_dbConn.Open();
                    SqliteCommand selectSQL = new SqliteCommand("SELECT * from character", m_dbConn);
                    SqliteDataReader reader = selectSQL.ExecuteReader();
                    res = new Dictionary<string, object>();
                    while (reader.Read())
                    {
                        Dictionary<string, object> character = new Dictionary<string, object>();
                        character.Add("name", reader["name"]);
                        character.Add("level", reader["level"]);
                        character.Add("race", reader["race"]);
                        character.Add("class", reader["class"]);
                        res.Add(ii.ToString(), character);
                        ii++;
                    }
                }
            } 
            catch(Exception e)
            {
                throw new Exception("Could not gather characters");
            }
            return res;
        }

        //Search a character by name 
        Dictionary<string, object> searchCharacter(string name)
        {
            Dictionary<string, object> res = null;
            try
            {
                //connect to database
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=MyDatabase.sqlite;Version=3;"))
                {
                    m_dbConn.Open();
                    //Check to see if user exists
                    SqliteCommand selectSQL = new SqliteCommand("SELECT COUNT(*) AS TotalNORows,* from character WHERE lower(name) = lower(?)", m_dbConn);
                    selectSQL.Parameters.Add(new SqliteParameter("name", name));
                    SqliteDataReader reader = selectSQL.ExecuteReader();
                    //Make sure no. rows is above 1
                    if (Convert.ToInt32(reader["TotalNORows"]) == 1)
                    {
                        while (reader.Read())
                        {
                            //Add user to dictionary
                            res = new Dictionary<string, object>();
                            res.Add("name", reader["name"]);
                            res.Add("age", reader["age"]);
                            res.Add("gender", reader["gender"]);
                            res.Add("biography", reader["biography"]);
                            res.Add("level", reader["level"]);
                            res.Add("race", reader["race"]);
                            res.Add("class", reader["class"]);
                            res.Add("constitution", reader["constitution"]);
                            res.Add("dexterity", reader["dexterity"]);
                            res.Add("strength", reader["strength"]);
                            res.Add("charisma", reader["charisma"]);
                            res.Add("inteligence", reader["inteligence"]);
                            res.Add("wisdom", reader["wisdom"]);
                            return res;
                        }
                    }
                    else
                    {
                        //Throw error if not found user
                        throw new Exception("No user found!");
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception("Could not find the character: "+ e.Message);
            }
            return res;
        }

        //Add a character to the database
        int addCharacter(string name, int age, string gender, string biography, int level, string race, string cclass, int constitution, int dexterity, int strength, int charisma, int inteligence, int wisdom)
        {
            try
            {
                //connect to database
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=MyDatabase.sqlite;Version=3;"))
                {
                    m_dbConn.Open();
                    //Check to see if user exists
                    SqliteCommand selectSQL = new SqliteCommand("SELECT COUNT(*) AS TotalNORows,* from character WHERE lower(name) = lower(?)", m_dbConn);
                    selectSQL.Parameters.Add(new SqliteParameter("name", name));
                    SqliteDataReader reader = selectSQL.ExecuteReader();
                    //Make sure no. rows is above 0
                    if (Convert.ToInt32(reader["TotalNORows"]) == 0)
                    {
                        //Add new character to the database
                        SqliteCommand insertSQL = new SqliteCommand("INSERT INTO character (name,age,gender,biography,level,race,class,constitution,dexterity,strength,charisma,inteligence,wisdom) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?)", m_dbConn);
                        insertSQL.Parameters.Add(new SqliteParameter("name", name));
                        insertSQL.Parameters.Add(new SqliteParameter("age", age));
                        insertSQL.Parameters.Add(new SqliteParameter("gender", gender));
                        insertSQL.Parameters.Add(new SqliteParameter("biography", biography));
                        insertSQL.Parameters.Add(new SqliteParameter("level", level));
                        insertSQL.Parameters.Add(new SqliteParameter("race", race));
                        insertSQL.Parameters.Add(new SqliteParameter("class", cclass));
                        insertSQL.Parameters.Add(new SqliteParameter("constitution", constitution));
                        insertSQL.Parameters.Add(new SqliteParameter("dexterity", dexterity));
                        insertSQL.Parameters.Add(new SqliteParameter("strength", strength));
                        insertSQL.Parameters.Add(new SqliteParameter("charisma", charisma));
                        insertSQL.Parameters.Add(new SqliteParameter("inteligence", inteligence));
                        insertSQL.Parameters.Add(new SqliteParameter("wisdom", wisdom));
                        insertSQL.ExecuteNonQuery();
                        return 0;
                    }
                    else
                    {
                        throw new Exception("Name has been taken!");
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception("Could not create character: " + e.Message);
            }
        }

        int editCharacter(string name, int age, string gender, string biography, int level, string race, string cclass, int constitution, int dexterity, int strength, int charisma, int inteligence, int wisdom)
        {
            try
            {
                //connect to database
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=MyDatabase.sqlite;Version=3;"))
                {
                    m_dbConn.Open();
                    //Check to see if user exists
                    SqliteCommand checkAmount = new SqliteCommand("SELECT COUNT(*) AS TotalNORows from character WHERE lower(name) = lower(?)", m_dbConn);
                    checkAmount.Parameters.Add(new SqliteParameter("name", name));
                    SqliteDataReader reader = checkAmount.ExecuteReader();
                    //Make sure no. rows is above 1
                    if (Convert.ToInt32(reader["TotalNORows"]) == 1)
                    {
                        //Update character info
                        SqliteCommand insertSQL = new SqliteCommand("UPDATE character SET age = ?,gender = ?,biography = ?,level = ?,race = ?,class = ?,constitution = ?,dexterity = ?,strength = ?,charisma = ?,inteligence = ?,wisdom = ? WHERE name = ?", m_dbConn);
                        insertSQL.Parameters.Add(new SqliteParameter("age", age));
                        insertSQL.Parameters.Add(new SqliteParameter("gender", gender));
                        insertSQL.Parameters.Add(new SqliteParameter("biography", biography));
                        insertSQL.Parameters.Add(new SqliteParameter("level", level));
                        insertSQL.Parameters.Add(new SqliteParameter("race", race));
                        insertSQL.Parameters.Add(new SqliteParameter("class", cclass));
                        insertSQL.Parameters.Add(new SqliteParameter("constitution", constitution));
                        insertSQL.Parameters.Add(new SqliteParameter("dexterity", dexterity));
                        insertSQL.Parameters.Add(new SqliteParameter("strength", strength));
                        insertSQL.Parameters.Add(new SqliteParameter("charisma", charisma));
                        insertSQL.Parameters.Add(new SqliteParameter("inteligence", inteligence));
                        insertSQL.Parameters.Add(new SqliteParameter("wisdom", wisdom));
                        insertSQL.Parameters.Add(new SqliteParameter("name", name));
                        insertSQL.ExecuteNonQuery();
                        return 0;
                    }
                    else
                    {
                        throw new Exception("No user found!");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not edit character: " + e.Message);
            }
        }

        //Delete character from the database
        void deleteCharacter(string name)
        {
            try
            {
                //connect to database
                using (SqliteConnection m_dbConn = new SqliteConnection("Data Source=MyDatabase.sqlite;Version=3;"))
                {
                    m_dbConn.Open();
                    //Check to see if user exists
                    SqliteCommand checkAmount = new SqliteCommand("SELECT COUNT(*) AS TotalNORows from character WHERE lower(name) = lower(?)", m_dbConn);
                    checkAmount.Parameters.Add(new SqliteParameter("name", name));
                    SqliteDataReader reader = checkAmount.ExecuteReader();
                    //Make sure no. rows is above 1
                    if (Convert.ToInt32(reader["TotalNORows"]) == 1)
                    {
                        //Delete the character
                        SqliteCommand insertSQL = new SqliteCommand("DELETE FROM character WHERE name = ?", m_dbConn);
                        insertSQL.Parameters.Add(new SqliteParameter("name", name));
                        insertSQL.ExecuteNonQuery();
                    }
                    else
                    {
                        throw new Exception("No user found!");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not delete character: " + e.Message);
            }
        }

        void checkEmpty(string name)
        {
            //check to make sure not empty
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Name cannot be left empty");
            }
        }

        void checkAge(int age)
        {
            //check to make sure the age is above 0 but below 500
            if (age > 500 || age < 0)
            {
                throw new Exception("Age must be between 0 - 500");
            }
        }

        void checkBio(string bio)
        {
            //check to make sure the bio is less than 500 characters
            if (bio.Length > 500)
            {
                throw new Exception("Biography must be below 500 characters");
            }
        }

        void checkLevel(int level)
        {
            //check to make sure the level is above 1 but below 20
            if (level > 20 || level < 1)
            {
                throw new Exception("Level must be between 1 - 20");
            }
        }


        void checkRC(string url, string type)
        {
            //Gathers the list of races or classes
            Dictionary<string, object> typeList = getUrlList(url);
            bool inList = false;
            //Places it in an array
            JArray typeArray = JArray.Parse(typeList["results"].ToString());

            //Checks to see if in array
            for (int ii = 0; ii < typeArray.Count; ii++)
            {
                if (typeArray[ii]["name"].ToString() == type)
                {
                    inList = true;
                }
            }

            //If not in array throw an error as they havent selected right one
            if (!inList)
            {
                throw new Exception("Invalid Race or class selected");
            }
        }

        void checkAbilities(int constitution, int dexterity, int strength, int charisma, int inteligence, int wisdom)
        {
            //check to make sure all the ability scores added up are 20
            int ability_score = constitution + dexterity + strength + charisma + inteligence + wisdom;
            if (ability_score < 20 || ability_score > 20)
            {
                throw new Exception("Ability Score must add up to 20");
            }
        }

        private string sanatiseData(string res)
        {
            string clean = "?&^$#@!()+-;<>’\'*";

            foreach (char c in clean)
            {
                res = res.Replace(c.ToString(), string.Empty);
            }
            return res;
        }
    }
}
