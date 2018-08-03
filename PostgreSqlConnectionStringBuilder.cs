using System;
using System.Data.Common;

namespace WahsKeyClubSite
{
    public enum SslMode
    {
        Require,
        Disable,
        Prefer
    }

    public class PostgreSqlConnectionStringBuilder : DbConnectionStringBuilder
    {
        private string database;
        private string host;
        private string password;
        private bool pooling;
        private int port;
        private string username;
        private bool trustServerCertificate;
        private SslMode sslMode;

        public PostgreSqlConnectionStringBuilder(string uriString)
        {
            ParseUri(uriString);
        }

        public string Database
        {
            get => database;
            set
            {
                base["database"] = value;
                database = value;
            }
        }

        public string Host
        {
            get => host;
            set
            {
                base["host"] = value;
                host = value;
            }
        }

        public string Password
        {
            get => password;
            set
            {
                base["password"] = value;
                password = value;
            }
        }

        public bool Pooling
        {
            get => pooling;
            set
            {
                base["pooling"] = value;
                pooling = value;
            }
        }

        public int Port
        {
            get => port;
            set
            {
                base["port"] = value;
                port = value;
            }
        }

        public string Username
        {
            get => username;
            set
            {
                base["username"] = value;
                username = value;
            }
        }

        public bool TrustServerCertificate
        {
            get => trustServerCertificate;
            set
            {
                base["trust server certificate"] = value;
                trustServerCertificate = value;
            }
        }

        public SslMode SslMode
        {
            get => sslMode;
            set
            {
                base["ssl mode"] = value.ToString();
                sslMode = value;
            }
        }

        public override object this[string keyword]
        {
            get
            {
                if(keyword == null) throw new ArgumentNullException(nameof(keyword));
                return base[keyword.ToLower()];
            }
            set
            {
                if(keyword == null) throw new ArgumentNullException(nameof(keyword));

                switch(keyword.ToLower())
                {
                    case "host":
                        Host = (string) value;
                        break;

                    case "port":
                        Port = Convert.ToInt32(value);
                        break;

                    case "database":
                        Database = (string) value;
                        break;

                    case "username":
                        Username = (string) value;
                        break;

                    case "password":
                        Password = (string) value;
                        break;

                    case "pooling":
                        Pooling = Convert.ToBoolean(value);
                        break;

                    case "trust server certificate":
                        TrustServerCertificate = Convert.ToBoolean(value);
                        break;

                    case "sslmode":
                        SslMode = (SslMode) value;
                        break;

                    default:
                        throw new ArgumentException(string.Format("Invalid keyword '{0}'.", keyword));
                }
            }
        }

        public override bool ContainsKey(string keyword) => base.ContainsKey(keyword.ToLower());

        private void ParseUri(string uriString)
        {
            var isUri = Uri.TryCreate(uriString, UriKind.Absolute, out var uri);

            if(!isUri) throw new FormatException(string.Format("'{0}' is not a valid URI.", uriString));

            Host = uri.Host;
            Port = uri.Port;
            Database = uri.LocalPath.Substring(1);
            Username = uri.UserInfo.Split(':')[0];
            Password = uri.UserInfo.Split(':')[1];
        }
    }
}