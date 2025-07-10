﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using DotNetEnv;

namespace GM_Dumago_Private_Resort_Admin_Desktop_Application
{
    public static class MongoDbService
    {
        private static IMongoDatabase database;

        public static void Initialize()
        {
            try
            {
                Env.Load();
                
                string connectionUri = Env.GetString("DB_CONNECTION_STRING");

                var settings = MongoClientSettings.FromConnectionString(connectionUri);
                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                var client = new MongoClient(settings);

                database = client.GetDatabase("ResortAdmin");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to the database. Please check your connection string and internet connection.\n\nError: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }


        public static IMongoCollection<BsonDocument> Admins
        {
            get { return database.GetCollection<BsonDocument>("admins"); }
        }

        public static IMongoCollection<BsonDocument> Customers
        {
            get { return database.GetCollection<BsonDocument>("customers"); }
        }

        public static IMongoCollection<BsonDocument> Reservations
        {
            get { return database.GetCollection<BsonDocument>("reservations"); }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}