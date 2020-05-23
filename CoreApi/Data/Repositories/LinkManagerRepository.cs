using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Commons;
using CoreApi.Extensions;
using CoreApi.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;
using System;
namespace CoreApi.Data.Repositories
{
    public interface ILinkManagerRepository
    {
       string GetLinkManager(string empcode);

    
    }
    public class LinkManagerRepository : ILinkManagerRepository
    {

       
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public LinkManagerRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }
        public string GetLinkManager(string empcode)
        {
            string KQ = string.Empty;
            string SQL = "PR_GET_FUNC_ADMIN";
            DataSet ds = new DataSet();
           

            try
            {
                using (var cnn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (var cmd = new SqlCommand(SQL, cnn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 30;

                        //cnn.OpenAsync();

                        // Discover Stored Procedure parameters
                        // SqlCommandBuilder.DeriveParameters(cmd);
                        cmd.Parameters.Add("@EMPCODE", SqlDbType.NVarChar, 2000);
                        cmd.Parameters["@EMPCODE"].Value = empcode;


                        SqlDataAdapter sda = new SqlDataAdapter();
                        sda.SelectCommand = cmd;
                        sda.Fill(ds, "DATA");
                    }
                }
                //==========================
                if(ds.Tables.Count>0)
                {
                    if(ds.Tables[0].Rows.Count>0)
                    {
                        if (int.Parse(ds.Tables[0].Rows[0]["TONG"].ToString()) > 0)
                        {
                            KQ = Cryptography.Encrypt(empcode,Cryptography.KLB_KeyOption());
                        }
                        else
                        {
                            KQ = "0";
                        }
                    }
                    else
                    {
                        KQ = "0";
                    }
                }
                else
                {
                    KQ = "0";
                }
            }
            catch
            {
                KQ = "0";

            }

            return KQ;
        }
    }
    public class Cryptography
    {
        public static string KLB_KeyOption()
        {
            return "P1Y4rVA3flK+63aJxxEYNA==";
        }
        public static string Encrypt(string toEncrypt, string key)
        {
            byte[] toEncryptArray = Encoding.UTF32.GetBytes(toEncrypt);

            // Get the key from config file
            //string key = (string)settingsReader.GetValue("SecurityKey",typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF32.GetBytes(key));
            //Always release the resources and flush data
            // of the Cryptographic service provide. Best Practice

            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes =
                new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
                cTransform.TransformFinalBlock(toEncryptArray, 0,
                                               toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, string key)
        {
            string result = "";
            //get the byte code of the string
            try
            {
                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                //Get your key from config file to open the lock!
                //string key = (string)settingsReader.GetValue("SecurityKey",typeof(String));

                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                byte[] keyArray = hashmd5.ComputeHash(Encoding.UTF32.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();

                TripleDESCryptoServiceProvider tdes =
                    new TripleDESCryptoServiceProvider();
                //set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;
                //mode of operation. there are other 4 modes. 
                //We choose ECB(Electronic code Book)

                tdes.Mode = CipherMode.ECB;
                //padding mode(if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(
                    toEncryptArray, 0, toEncryptArray.Length);
                //Release resources held by TripleDes Encryptor                
                tdes.Clear();
                //return the Clear decrypted TEXT
                result = Encoding.UTF32.GetString(resultArray);
            }
            catch (Exception ex)
            {
                string res = ex.Message;
            }
            return result;
        }

        public static string DecryptAuthenKey(string data, string key)
        {
            data = "ws1" + data;
            byte[] databyte;
            byte[] keybyte;
            databyte = Encoding.UTF8.GetBytes(data.ToCharArray());
            keybyte = Encoding.UTF8.GetBytes(key);
            HMACSHA1 hash = new HMACSHA1(keybyte);
            return Convert.ToBase64String(hash.ComputeHash(databyte));
        }
    }
}
