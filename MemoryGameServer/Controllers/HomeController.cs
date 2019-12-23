using MemoryGameServer.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MemoryGameServer.Controllers
{
    public class HomeController : Controller
    {
        public const string CONNECTION_STRING = "Server= DESKTOP-C94M5QS;" +
                                                "Database=Team8MemoryGame;" +
                                                "Integrated Security=true";

        public JsonResult getPlayer() {

            List<Player> p = new List<Player>();
            p = getAllPlayer();

            Debug.WriteLine(p);

            return Json(p, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult setPlayer(Player player) {
            int maxid = 0;
            int newid = 0;

            if (player != null) {

                maxid = maxId();
                newid = maxid + 1;

                Player newPlayer = new Player();
                newPlayer.PlayerId = newid;
                newPlayer.PlayerName = player.PlayerName;
                newPlayer.Time = player.Time;

                Console.WriteLine(newPlayer);

                updatePlayer(newPlayer);

                Console.WriteLine("Name:" + player.PlayerName + "Time:" + player.Time);
                return Json(new { status = "ok" });
            }
            return Json(new { status = "fail" });

        }

        public List<Player> getAllPlayer() {

            List<Player> players = new List<Player>();

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING)) {

                connection.Open();

                string queryString = @"SELECT TOP 5 PlayerName, Time FROM Players" +
                    " WHERE Time>0 ORDER BY Time";
                SqlCommand command = new SqlCommand(queryString, connection);
                SqlDataReader r = command.ExecuteReader();

                while (r.HasRows) 
                {
                    Debug.WriteLine("\t{0}\t{1}", r.GetName(0), 
                        r.GetName(1));

                    while (r.Read())
                    {
                        Debug.WriteLine("\t{0}\t{1}", r.GetString(0),
                            r.GetInt32(1));

                        Player p = new Player()
                        {
                            PlayerName = r.GetString(0),
                            Time = r.GetInt32(1),
                        };
                        players.Add(p);
                    }
                    r.NextResult();
                }
            }
            return players;
        }

        public void updatePlayer(Player player) {

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                
                string queryString = @"INSERT INTO Players (PlayerId, PlayerName, Time)
                                    VALUES(@PlayerId, @PlayerName, @Time)";
                SqlParameter playerid = new SqlParameter();
                playerid.ParameterName = "@PlayerId";
                playerid.Value = player.PlayerId;

                SqlParameter playername = new SqlParameter();
                playername.ParameterName = "@playerName";
                playername.Value = player.PlayerName;

                SqlParameter time = new SqlParameter();
                time.ParameterName = "@Time";
                time.Value = player.Time;

                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add(playerid);
                command.Parameters.Add(playername);
                command.Parameters.Add(time);
                command.ExecuteNonQuery();

            }

        }

        public int maxId() {
            int maxid = 0;

            using (SqlConnection connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                string queryString = @"SELECT MAX(PlayerId) FROM Players";
                SqlCommand command = new SqlCommand(queryString, connection);
                maxid = (int)command.ExecuteScalar();
            }
            return maxid;
        }
        public JsonResult SetPerson()
        {
            
            List<Person> waitingList = (List<Person>)HttpContext.Application["waitingList"];
            
            /*if (waitingList == null || waitingList.Count == 0)
            {
                person.name = "Guest1";
            }
            else
            {*/
            //Person lastPerson = waitingList.LastOrDefault();
            //String indexLastPerson = lastPerson.name.Substring(5);
            HttpContext.Application["userCounter"]= (int)HttpContext.Application["userCounter"]+1;
            Person person = new Person();
            person.name = "Guest" + ((int)HttpContext.Application["userCounter"]);
           //}/**/

            if (person != null && person.name != null)
            {
                waitingList.Add(person);
                return Json(person, JsonRequestBehavior.AllowGet);
                
            }
            return Json(new { status = "fail" });
        }
   

        public JsonResult MatchPerson(Person person)
        {
            //if there is no data passed
            if (person == null || person.name == null || person.name.Equals(""))
            {
                Debug.WriteLine("No data passed to Match Person");
                return Json(new { status = "no match" });
            }
            //if the person is in the waitinglist and the waitinglist is not empty: match and move them to match list
            List<Person> waitingList = (List<Person>)HttpContext.Application["waitingList"];//reference
            Debug.WriteLine("the waiting list is " + waitingList.Count);

            Debug.WriteLine("the current player is in the list?" + waitingList.Contains(person));
            List<Match> matchList = (List<Match>)HttpContext.Application["matchList"];//reference

            if (waitingList != null && waitingList.Count != 0 && waitingList.Contains(person))
            {
                Debug.WriteLine("The current player is in the waitingList");

                foreach (Person p in waitingList)
                {
                    if (!person.name.Equals(p.name))
                    {
                        Debug.WriteLine("found a match in waitlist");
                        //move both to match
                        Match match = new Match();
                        match.Player1 = person;
                        match.Player2 = p;
                        matchList.Add(match);
                        waitingList.Remove(p);
                        waitingList.Remove(person);
                        return Json(p, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else if (!waitingList.Contains(person))
            {
                Debug.WriteLine("The current player is not in the waitinglist");
                //if the person is not in the waiting list, check if in the matchlist and return the opponent name
                foreach (Match m in matchList)
                {
                    if (m.Player1.Equals(person))
                    {
                        Debug.WriteLine("the player in a match with" + m.Player2.name);
                        return Json(m.Player2, JsonRequestBehavior.AllowGet);
                    }
                    else if (m.Player2.Equals(person))
                    {
                        Debug.WriteLine("the player in a match with" + m.Player1.name);
                        return Json(m.Player1, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return Json(new { status = "no match" });

        }
        public JsonResult RemovePerson(Person person)
        {
            if (person == null || person.name == null)
            {
                return Json(new { status = "no person to remove" });
            }

            List<Person> waitingList = (List<Person>)HttpContext.Application["waitingList"];//value or reference?
            if (waitingList != null && waitingList.Count != 0 && person != null)
            {
                foreach (Person p in waitingList)
                {
                    if (person.name.Equals(p.name))
                    {
                        waitingList.Remove(p);
                        return Json(p, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { status = "no match" });

        }

        public JsonResult StartGame(Person p)
        {

            //get the application variable gameList and matchlist
            List<Game> gameList = (List<Game>)HttpContext.Application["gameList"];//reference
            List<Match> matchList = (List<Match>)HttpContext.Application["matchList"];//reference
            //locate the match
            foreach (Match m in matchList)
            {
                if (m.Player1.Equals(p) || m.Player2.Equals(p))
                {
                    Game game = new Game();
                    game.match = m;
                    gameList.Add(game);
                    matchList.Remove(m);
                    return Json(new { status = "started" });
                }
            }
            //move the match to gamelist
            //return status started
            return Json(new { status = "no such person in match" });
        }

        public JsonResult IsStart(Person p)
        {
            //get the application variable gameList and matchlist
            List<Game> gameList = (List<Game>)HttpContext.Application["gameList"];//reference

            //try to find the person in the gamelist: if found true; else false
            foreach (Game g in gameList)
            {
                if (g.match.Player1.Equals(p) || g.match.Player2.Equals(p))
                {
                    return Json(new { status = "true" });
                }
            }
            return Json(new { status = "false" });
        }
        public JsonResult FinishGame(Person p)
        {
            //get the application variable gameList and matchlist
            List<Game> gameList = (List<Game>)HttpContext.Application["gameList"];//reference
            //try to find the person in the gamelist: if found true; else false
            foreach (Game g in gameList)
            {
                if (g.match.Player1.Equals(p) || g.match.Player2.Equals(p))
                {
                    if (g.winner == null)
                    {
                        g.winner = p;
                        return Json(new { status = "Ok" });
                    }
                    else
                    {
                        return Json(new { status = "HasWinner" });
                    }
                }
            }
            return Json(new { status = "fail" });
        }
        public JsonResult GetWinner(Person p)
        {
            //get the application variable gameList and matchlist
            List<Game> gameList = (List<Game>)HttpContext.Application["gameList"];//reference         
            //try to find the person in the gamelist: if found true; else false
            foreach (Game g in gameList)
            {
                if (g.match.Player1.Equals(p) || g.match.Player2.Equals(p))
                {
                    if (g.winner == null)
                    {
                        return Json(new { status = "NoWinner" });
                    }
                    else
                    {
                        Person winner = g.winner;
                        return Json(winner, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { status = "fail" });
        }
        public JsonResult RemoveGame(Person p)
        {
            //get the application variable gameList and matchlist
            List<Game> gameList = (List<Game>)HttpContext.Application["gameList"];//reference         
            //try to find the person in the gamelist: if found true; else false
            foreach (Game g in gameList)
            {
                if (g.match.Player1.Equals(p) || g.match.Player2.Equals(p))
                {
                    gameList.Remove(g);
                    return Json(new { status = "GameRemoved" });
                }
            }
            return Json(new { status = "fail" });
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}