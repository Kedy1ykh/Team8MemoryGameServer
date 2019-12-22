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