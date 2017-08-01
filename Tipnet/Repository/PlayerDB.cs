using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using Tipnet.DTOs;
using Tipnet.Models;

namespace Tipnet.Repository
{
    public class PlayerDB
    {
        private MySqlConnection conObj;

        private string conStr = "SERVER='localhost';"
                                + "DATABASE='tipnet';"
                                + "UID='root';"
                                + "PASSWORD='';"
                                + "Convert Zero Datetime=True;";


        public bool InsertPlayer(Player player)
        {
            conObj = new MySqlConnection(conStr);

            string insQuery = "INSERT INTO players "
                              +
                              "VALUES (@Id, @Ime, @Prezime, @Password, @Salt, @RacunBlokiran, @PogresanPass, @VrijemeBlokade," +
                              " @Email, @EmailConfirmed, @Guid, @PrGuid, @PrTimeStamp, @SessionGuid, @DatumRodjenja, @Ulica, @KucniBroj, @Grad," +
                              " @PostanskiBroj, @Drzava, @Jezik, @BrojTelefona, @BrojMobitela, @Oslovljavanje, @Username, @Role, @McGuid, @McTimeStamp," +
                              " @BlockDuration, @BlockReason, @BlockExplanation, @BlockTimeStamp, @AutoOdjava, @AoTimeStamp, @AoFlag, @PauzaKladenja,"+
                              " @PkTimeStamp,@PkFlag, @RacunVerificiran)";

            MySqlCommand com = new MySqlCommand(insQuery, conObj);
            com.Parameters.AddWithValue("@Id", null);
            com.Parameters.AddWithValue("@Ime", player.Ime);
            com.Parameters.AddWithValue("@Prezime", player.Prezime);
            com.Parameters.AddWithValue("@Password", player.Password);
            com.Parameters.AddWithValue("@Salt", player.Salt);
            com.Parameters.AddWithValue("@RacunBlokiran", 0);
            com.Parameters.AddWithValue("@PogresanPass", 0);
            com.Parameters.AddWithValue("@VrijemeBlokade", DateTime.MinValue);
            com.Parameters.AddWithValue("@Email", player.Email);
            com.Parameters.AddWithValue("@EmailConfirmed", player.EmailConfirmed);
            com.Parameters.AddWithValue("@Guid", null);
            com.Parameters.AddWithValue("@PrGuid", null);
            com.Parameters.AddWithValue("@PrTimeStamp", DateTime.MinValue);
            com.Parameters.AddWithValue("@SessionGuid", null);
            com.Parameters.AddWithValue("@DatumRodjenja", player.DatumRodenja);
            com.Parameters.AddWithValue("@Ulica", player.Ulica);
            com.Parameters.AddWithValue("@KucniBroj", player.KucniBroj);
            com.Parameters.AddWithValue("@Grad", player.Grad);
            com.Parameters.AddWithValue("@PostanskiBroj", player.PostanskiBroj);
            com.Parameters.AddWithValue("@Drzava", player.Drzava);
            com.Parameters.AddWithValue("@Jezik", player.Jezik);
            com.Parameters.AddWithValue("@BrojTelefona", player.BrojTelefona);
            com.Parameters.AddWithValue("@BrojMobitela", player.BrojMobilnog);
            com.Parameters.AddWithValue("@Oslovljavanje", player.Oslovljavanje);
            com.Parameters.AddWithValue("@Username", player.Username);
            com.Parameters.AddWithValue("@Role", "player");
            com.Parameters.AddWithValue("@McGuid", null);
            com.Parameters.AddWithValue("@McTimeStamp", DateTime.MinValue);
            com.Parameters.AddWithValue("@BlockDuration", 0);
            com.Parameters.AddWithValue("@BlockReason", 0);
            com.Parameters.AddWithValue("@BlockExplanation", 0);
            com.Parameters.AddWithValue("@BlockTimeStamp", DateTime.MinValue);
            com.Parameters.AddWithValue("@AutoOdjava", 0);
            com.Parameters.AddWithValue("@AoTimeStamp", DateTime.MinValue);
            com.Parameters.AddWithValue("@AoFlag", false);
            com.Parameters.AddWithValue("@PauzaKladenja", 0);
            com.Parameters.AddWithValue("@PkTimeStamp", DateTime.MinValue);
            com.Parameters.AddWithValue("@PkFlag", false);
            com.Parameters.AddWithValue("@RacunVerificiran", false);

            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();

            if (i >= 1)
            {
                return true;
            }

            return false;

        }

        internal string GetSessionGuid(string username)
        {
            string sessionGuid = "";
            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT sessionGuid FROM `players` WHERE username = @Username";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                
                sessionGuid = Convert.ToString(dr["sessionGuid"]);

                return sessionGuid;
            }

            return sessionGuid;
        }

        public List<UsernameAndEmailDTO> GetAllUsernameAndEmails()
        {
            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT email, emailConfirmed, racunBlokiran, username FROM `players`";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();
            List<UsernameAndEmailDTO> UsernameAndEmailList = new List<UsernameAndEmailDTO>();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                UsernameAndEmailDTO UM = new UsernameAndEmailDTO();
                UM.Email = Convert.ToString(dr["email"]);
                UM.Username = Convert.ToString(dr["username"]);
                UM.EmailConfirmed = Convert.ToBoolean(dr["emailConfirmed"]);
                UM.AccDisabled = Convert.ToBoolean(dr["racunBlokiran"]);
                UsernameAndEmailList.Add(UM);
            }

            return UsernameAndEmailList;
        }

        public List<Drzava> GetCountries()
        {
            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT id, country_code, country_name FROM `apps_countries`";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();
            List<Drzava> ListaDrzava = new List<Drzava>();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                var Drzava = new Drzava();
                Drzava.Id = Convert.ToInt32(dr["id"]);
                Drzava.Code = Convert.ToString(dr["country_code"]);
                Drzava.Ime = Convert.ToString(dr["country_name"]);
                ListaDrzava.Add(Drzava);
            }

            return ListaDrzava;
        }

        public SaltAndPassDTO GetSaltAndPassword(string username)
        {
            SaltAndPassDTO SaltAndPass = null;

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT salt, password, pogresanPassword, vrijemeBlokade FROM `players` WHERE username = @Username";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];

                SaltAndPass = new SaltAndPassDTO();

                SaltAndPass.Salt = Convert.ToString(dr["salt"]);
                SaltAndPass.Password = Convert.ToString(dr["password"]);
                SaltAndPass.PogresanPass = Convert.ToInt32(dr["pogresanPassword"]);
                SaltAndPass.VrijemeBlokade = Convert.ToDateTime(dr["vrijemeBlokade"]);

                return SaltAndPass;
            }

            return null;
        }

        public bool KriviPassword(int brojPogresnihPokusaja, string username)
        {

            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET pogresanPassword=@PogresanPass " +
                            "WHERE username=@Username";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@PogresanPass", brojPogresnihPokusaja);
            com.Parameters.AddWithValue("@Username", username);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;

        }

        public bool DodajVrijemeBlokade(string username)
        {

            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET vrijemeBlokade=@VrijemeBlokade " +
                            "WHERE username=@Username";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@VrijemeBlokade", DateTime.Now);
            com.Parameters.AddWithValue("@Username", username);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;

        }

        public bool NoviMailGuid(string guid, string email)
        {

            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET guid=@Guid " +
                            "WHERE email=@Email";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@Guid", guid);
            com.Parameters.AddWithValue("@Email", email);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;

        }

        public List<string> GetAllGuids()
        {
            var guidList = new List<string>();

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT guid FROM `players`";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                guidList.Add(Convert.ToString(dr["guid"]));

            }

            return guidList;
        }

        public bool ConfirmAcc(string guid)
        {

            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET emailConfirmed=1 " +
                            "WHERE guid=@Guid";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@Guid", guid);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;

        }

        public BirthdateAndUsername GetBirthdate(string email)
        {
            var birthDateAndUsername = new BirthdateAndUsername();

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT datumRodjenja, username FROM `players` WHERE email = @Email";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Email", email);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                birthDateAndUsername.DatumRodenja = Convert.ToDateTime(dr["datumRodjenja"]);
                birthDateAndUsername.Username = Convert.ToString(dr["username"]);

                return birthDateAndUsername;
            }

            return null;
        }

        public BirthdateAndEmail GetBirthdateAndEmail(string username)
        {
            var birthDateAndEmail = new BirthdateAndEmail();

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT datumRodjenja, email FROM `players` WHERE username = @Username";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                birthDateAndEmail.DatumRodenja = Convert.ToDateTime(dr["datumRodjenja"]);
                birthDateAndEmail.Email = Convert.ToString(dr["email"]);

                return birthDateAndEmail;
            }

            return null;
        }


        public bool PasswordResetInit(string prGuid, string email)
        {

            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET prGuid=@PrGuid, prTimeStamp=@PrTimeStamp " +
                            "WHERE email=@Email";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@PrGuid", prGuid);
            com.Parameters.AddWithValue("@PrTimeStamp", DateTime.Now);
            com.Parameters.AddWithValue("@Email", email);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;

        }

        public PasswordResetDTO GetPasswordResetInfo(string key)
        {
            var passwordResetInfo = new PasswordResetDTO();

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT prTimeStamp, username FROM `players` WHERE prGuid=@key";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@key", key);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                passwordResetInfo.PrTimeStamp = Convert.ToDateTime(dr["prTimeStamp"]);
                passwordResetInfo.Username = Convert.ToString(dr["username"]);

                return passwordResetInfo;
            }

            return null;
        }

        public bool UpdatePassworda(string pass, string salt, string prGuid)
        {

            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET password=@Password, salt=@Salt " +
                            "WHERE prGuid=@PrGuid";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@Password", pass);
            com.Parameters.AddWithValue("@Salt", salt);
            com.Parameters.AddWithValue("@PrGuid", prGuid);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;

        }

        public bool UpdatePasswordaUsername(string pass, string salt, string username)
        {

            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET password=@Password, salt=@Salt " +
                            "WHERE username=@Username";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@Password", pass);
            com.Parameters.AddWithValue("@Salt", salt);
            com.Parameters.AddWithValue("@Username", username);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;

        }

        public EditDataDTO DohvatiPodatkeZaIzmjenu(string username, string sessionId)
        {
            var editData = new EditDataDTO();

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT ulica, kucniBroj, grad, postanskiBroj, drzavaId, jezik, brojTelefona," +
                " brojMobilnog, oslovljavanje FROM `players` WHERE username=@Username AND sessionGuid=@SessionId";

            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);
            com.Parameters.AddWithValue("@SessionId", sessionId);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                editData.Ulica = Convert.ToString(dr["ulica"]);
                editData.KucniBroj = Convert.ToString(dr["kucniBroj"]);
                editData.Grad = Convert.ToString(dr["grad"]);
                editData.PostanskiBroj = Convert.ToInt32(dr["postanskiBroj"]);
                editData.Drzava = Convert.ToString(dr["drzavaId"]);
                editData.Jezik = Convert.ToString(dr["jezik"]);
                editData.BrojTelefona = Convert.ToString(dr["brojTelefona"]);
                editData.BrojMobilnog = Convert.ToString(dr["brojMobilnog"]);
                editData.Oslovljavanje = Convert.ToString(dr["oslovljavanje"]);

                return editData;
            }

            return null;
        }

        public bool UpisiSessionId(string username, string sessionGuid)
        {

            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET sessionGuid=@SessionGuid " +
                            "WHERE username=@Username";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@SessionGuid", sessionGuid);
            com.Parameters.AddWithValue("@Username", username);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;

        }

        public bool EditData(EditDataDTO editedData, string username, string sessionGuid)
        {

            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET ulica=@Ulica, kucniBroj=@KucniBroj, grad=@Grad, postanskiBroj=@PostanskiBroj," +
                " drzavaId=@DrzavaId, jezik=@Jezik, brojTelefona=@BrojTelefona, brojMobilnog=@BrojMobilnog, oslovljavanje=@Oslovljavanje" +
                " WHERE username=@Username AND sessionGuid=@SessionId";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@Ulica", editedData.Ulica);
            com.Parameters.AddWithValue("@KucniBroj", editedData.KucniBroj);
            com.Parameters.AddWithValue("@Grad", editedData.Grad);
            com.Parameters.AddWithValue("@PostanskiBroj", editedData.PostanskiBroj);
            com.Parameters.AddWithValue("@DrzavaId", editedData.Drzava);
            com.Parameters.AddWithValue("@Jezik", editedData.Jezik);
            com.Parameters.AddWithValue("@BrojTelefona", editedData.BrojTelefona);
            com.Parameters.AddWithValue("@BrojMobilnog", editedData.BrojMobilnog);
            com.Parameters.AddWithValue("@Oslovljavanje", editedData.Oslovljavanje);
            com.Parameters.AddWithValue("@Username", username);
            com.Parameters.AddWithValue("@SessionId", sessionGuid);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;

        }


        // mozda ovo refaktorirati po nekom novom DTO tako da ne dohvacam sve podatke vec username role i jos nesto ako treba

        public Player DohvatiPlayera(string username)
        {
            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT `id`, `ime`, `prezime`, `password`, `salt`, `email`, `emailConfirmed`," +
                " `datumRodjenja`, `ulica`, `kucniBroj`," +
                " `grad`, `postanskiBroj`, `drzavaId`, `jezik`, `brojTelefona`, `brojMobilnog`, `oslovljavanje`, `username`, `role`" +
                " FROM `players` WHERE username=@Username";

            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];

                Player pl = new Player();

                pl.Id = Convert.ToInt32(dr["id"]);
                pl.Ime = Convert.ToString(dr["ime"]);
                pl.Prezime = Convert.ToString(dr["prezime"]);
                pl.Password = Convert.ToString(dr["password"]);
                pl.Salt = Convert.ToString(dr["salt"]);
                pl.Email = Convert.ToString(dr["email"]);
                pl.EmailConfirmed = Convert.ToBoolean(dr["emailConfirmed"]);
                pl.DatumRodenja = Convert.ToDateTime(dr["datumRodjenja"]);
                pl.Ulica = Convert.ToString(dr["ulica"]);
                pl.KucniBroj = Convert.ToString(dr["kucniBroj"]);
                pl.Grad = Convert.ToString(dr["grad"]);
                pl.PostanskiBroj = Convert.ToInt32(dr["postanskiBroj"]);
                pl.Drzava = Convert.ToInt32(dr["drzavaId"]);
                pl.Jezik = Convert.ToString(dr["jezik"]);
                pl.BrojTelefona = Convert.ToString(dr["brojTelefona"]);
                pl.BrojMobilnog = Convert.ToString(dr["brojMobilnog"]);
                pl.Oslovljavanje = Convert.ToString(dr["oslovljavanje"]);
                pl.Username = Convert.ToString(dr["username"]);
                pl.Role = Convert.ToString(dr["role"]);



                return pl;
            }

            return null;
        }

        // -------------------------------------   ZA ADMINA  ------------------------------

        public IEnumerable<Player> GetAllPlayers()
        {
            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT * FROM `players` WHERE 1";

            MySqlCommand com = new MySqlCommand(selQuery, conObj);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();
            List<Player> listaPlayera = new List<Player>();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                Player pl = new Player();

                pl.Id = Convert.ToInt32(dr["id"]);
                pl.Ime = Convert.ToString(dr["ime"]);
                pl.Prezime = Convert.ToString(dr["prezime"]);
                pl.Password = Convert.ToString(dr["password"]);
                pl.Salt = Convert.ToString(dr["salt"]);
                pl.Email = Convert.ToString(dr["email"]);
                pl.EmailConfirmed = Convert.ToBoolean(dr["emailConfirmed"]);
                pl.DatumRodenja = Convert.ToDateTime(dr["datumRodjenja"]);
                pl.Ulica = Convert.ToString(dr["ulica"]);
                pl.KucniBroj = Convert.ToString(dr["kucniBroj"]);
                pl.Grad = Convert.ToString(dr["grad"]);
                pl.PostanskiBroj = Convert.ToInt32(dr["postanskiBroj"]);
                pl.Drzava = Convert.ToInt32(dr["drzavaId"]);
                pl.Jezik = Convert.ToString(dr["jezik"]);
                pl.BrojTelefona = Convert.ToString(dr["brojTelefona"]);
                pl.BrojMobilnog = Convert.ToString(dr["brojMobilnog"]);
                pl.Oslovljavanje = Convert.ToString(dr["oslovljavanje"]);
                pl.Username = Convert.ToString(dr["username"]);
                pl.Role = Convert.ToString(dr["role"]);
                listaPlayera.Add(pl);
            }

            return listaPlayera;
        }


        // --------------- Promjena emaila -------------------------------------------

        public bool ChangeEmailInit(string mcGuid, string username)
        {
            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET mcGuid=@McGuid, mcTimeStamp=@McTimeStamp " +
                            "WHERE username=@Username";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@McGuid", mcGuid);
            com.Parameters.AddWithValue("@McTimeStamp", DateTime.Now);
            com.Parameters.AddWithValue("@Username", username);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;
        }

        public DateTime GetEmailChangeInfo(string mcGuid)
        {
            var emailChangeTimeStamp = new DateTime();

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT mcTimeStamp FROM `players` WHERE mcGuid=@key";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@key", mcGuid);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                emailChangeTimeStamp = Convert.ToDateTime(dr["mcTimeStamp"]);

                return emailChangeTimeStamp;
            }

            return DateTime.MinValue;
        }

        public bool ChangeMailFinish(string mcGuid, string email)
        {
            conObj = new MySqlConnection(conStr);
            string updStr = "UPDATE `players` SET email=@Email " +
                            "WHERE mcGuid=@McGuid";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@Email", email);
            com.Parameters.AddWithValue("@McGuid", mcGuid);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;
        }

        public string GetEmailByUsername(string username)
        {
            string email = null;

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT email FROM `players` WHERE username=@Username";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                email = Convert.ToString(dr["email"]);

                return email;
            }

            return email;
        }

        // -------------------------------------------------------------------------------------
        // -------------------------------  BLOKADA RACUNA   -----------------------------------


        public BlokadaRacunaDTO GetBlockData(string username)
        {
            var blockData = new BlokadaRacunaDTO();

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT racunBlokiran, blockDuration, blockReason, blockExplanation, blockTimeStamp FROM `players` WHERE username=@Username";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                blockData.RacunTrajnoBlokiran = Convert.ToBoolean(dr["racunBlokiran"]);
                blockData.TrajanjeBlokade = Convert.ToString(dr["blockDuration"]);
                blockData.RazlogBlokade = Convert.ToString(dr["blockReason"]);
                blockData.Obrazlozenje = Convert.ToString(dr["blockExplanation"]);
                blockData.TimeStampBlokade = Convert.ToDateTime(dr["blockTimeStamp"]);

                return blockData;
            }

            return blockData;
        }


        public bool SetBlockData(BlokadaRacunaDTO blokada, string username)
        {
            conObj = new MySqlConnection(conStr);

            string updStr = "UPDATE `players` SET racunBlokiran=@RacunBlokiran, blockDuration=@BlockDuration, blockReason=@BlockReason," +
                            " blockExplanation=@BlockExplanation, blockTimeStamp=@BlockTimeStamp" +
                            " WHERE username=@Username";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@RacunBlokiran", blokada.RacunTrajnoBlokiran);
            com.Parameters.AddWithValue("@BlockDuration", blokada.TrajanjeBlokade);
            com.Parameters.AddWithValue("@BlockReason", blokada.RazlogBlokade);
            com.Parameters.AddWithValue("@BlockExplanation", blokada.Obrazlozenje);
            com.Parameters.AddWithValue("@BlockTimeStamp", DateTime.Now);
            com.Parameters.AddWithValue("@Username", username);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;
        }
        // -------------------------------------------------------------------------------------
        // ----------------------------  AUTOMATSKA ODJAVA  ------------------------------------

        public TimeOffDTO GetTimeOffData(string username)
        {
            var timeOffData = new TimeOffDTO();

            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT autoOdjava, aoTimeStamp, aoFlag, pauzaKladenja, pkTimeStamp, pkFlag FROM `players` WHERE username=@Username";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];
                timeOffData.AutoOdjava = Convert.ToInt32(dr["autoOdjava"]);
                timeOffData.AutoOdjavaTimeStamp = Convert.ToDateTime(dr["aoTimeStamp"]);
                timeOffData.AutoOdjavaFlag = Convert.ToBoolean(dr["aoFlag"]);
                timeOffData.PauzaKladenja = Convert.ToInt32(dr["pauzaKladenja"]);
                timeOffData.PauzaKladenjaTimeStamp = Convert.ToDateTime(dr["pkTimeStamp"]);
                timeOffData.PauzaKladenjaFlag = Convert.ToBoolean(dr["pkFlag"]);

                return timeOffData;
            }

            return timeOffData;
        }

        public bool SetTimeOffData(TimeOffDTO timeOff, string username)
        {
            conObj = new MySqlConnection(conStr);

            string updStr = "UPDATE `players` SET autoOdjava=@AutoOdjava, aoTimeStamp=@AoTimeStamp, aoFlag=@AoFlag, pauzaKladenja=@PauzaKladenja," +
                            " pkTimeStamp=@PkTimeStamp, pkFlag=@PkFlag WHERE username=@Username";

            MySqlCommand com = new MySqlCommand(updStr, conObj);
            com.Parameters.AddWithValue("@AutoOdjava", timeOff.AutoOdjava);
            com.Parameters.AddWithValue("@AoTimeStamp", timeOff.AutoOdjavaTimeStamp);
            com.Parameters.AddWithValue("@AoFlag", timeOff.AutoOdjavaFlag);
            com.Parameters.AddWithValue("@PauzaKladenja", timeOff.PauzaKladenja);
            com.Parameters.AddWithValue("@PkTimeStamp", timeOff.PauzaKladenjaTimeStamp);
            com.Parameters.AddWithValue("@PkFlag", timeOff.PauzaKladenjaFlag);
            com.Parameters.AddWithValue("@Username", username);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;
        }

        // -------------------------------------------------------------------------------------
        // ----------------------------  UPLOAD DOKUMENTA  ------------------------------------

        public bool DodajDokument(byte[] array, string fileName, string username)
        {
            conObj = new MySqlConnection(conStr);

            string insQuery = "INSERT INTO `players_documents`(`id`, `slika`, `name`, `uploadTimeStamp`, `status`, `playerId`) " +
                "SELECT @Id, @Slika, @Name, @UploadTimeStamp, @Status, p.id FROM players p WHERE p.username =@Username  ";

            MySqlCommand com = new MySqlCommand(insQuery, conObj);
            com.Parameters.AddWithValue("@Id", null);
            com.Parameters.AddWithValue("@Slika", array);
            com.Parameters.AddWithValue("@Name", fileName);
            com.Parameters.AddWithValue("@UploadTimeStamp", DateTime.Now);
            com.Parameters.AddWithValue("@Status", false);
            com.Parameters.AddWithValue("@Username", username);
            conObj.Open();

            int i = com.ExecuteNonQuery();
            conObj.Close();
            if (i >= 1)
                return true;
            else
                return false;
        }

        public int DocumentCount(string username)
        {
            conObj = new MySqlConnection(conStr);

            string insQuery =
                "SELECT COUNT(*) FROM players_documents WHERE playerId=(SELECT id FROM players WHERE username =@Username)";

            MySqlCommand com = new MySqlCommand(insQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);
            conObj.Open();

            var oValue = com.ExecuteScalar();

            conObj.Close();

            if (oValue == DBNull.Value)
                return 0;

            return Convert.ToInt32(oValue);
        }

        public IEnumerable<ImageDTO> GetAllDocuments(string username)
        {
            conObj = new MySqlConnection(conStr);

            string selQuery = "SELECT slika, name, uploadTimeStamp, status FROM `players_documents` WHERE playerId=(SELECT id FROM players WHERE username =@Username)";

            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();
            var imgList = new List<ImageDTO>();

            da.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                var img = new ImageDTO();

                img.Ime = Convert.ToString(dr["name"]);
                img.Slika = (byte[])(dr["slika"]);
                img.UploadTimeStamp = Convert.ToDateTime(dr["uploadTimeStamp"]);
                img.Status = Convert.ToBoolean(dr["status"]);
                imgList.Add(img);
            }

            return imgList;
        }

        public bool KorisnikVerificiran(string username)
        {
            conObj = new MySqlConnection(conStr);
            var isVerified = false;

            string selQuery = "SELECT racunVerificiran FROM `players` WHERE username = @Username";
            MySqlCommand com = new MySqlCommand(selQuery, conObj);
            com.Parameters.AddWithValue("@Username", username);

            MySqlDataAdapter da = new MySqlDataAdapter(com);
            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count != 0)
            {
                DataRow dr = dt.Rows[0];

                isVerified = Convert.ToBoolean(dr["racunVerificiran"]);

                return isVerified;
            }

            return isVerified;
        }
    }
}