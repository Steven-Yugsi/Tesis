using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tesis.Conexion
{
    public class DBConn
    {
        public const string WepApyAuthentication = "AIzaSyBKKpL1sBe30OQUzwY8P9m-L4N3BhK0XMc";
    }
    public class Conexionfirebase
    {
        public static FirebaseClient firebase = new FirebaseClient("https://deteccion-2c411-default-rtdb.firebaseio.com/");
    }
}
