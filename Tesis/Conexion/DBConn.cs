using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tesis.Models;

namespace Tesis.Conexion
{
    public class DBConn
    {
        public const string WepApyAuthentication = "AIzaSyBKKpL1sBe30OQUzwY8P9m-L4N3BhK0XMc";
    }
    public class Conexionfirebase
    {
        public static FirebaseClient firebase = new FirebaseClient("https://deteccion-2c411-default-rtdb.firebaseio.com/");
        public static FirebaseStorage firebaseStorage = new FirebaseStorage("deteccion-2c411.firebasestorage.app",
         new FirebaseStorageOptions
         {
             ThrowOnCancel = true // Para manejar errores de cancelación
         });
    }

}
