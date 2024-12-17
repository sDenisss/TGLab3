using System.Numerics;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using TG.Commands.StartCommands;
using TG.DataBase;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;

namespace Tg
{
    public partial class Program // Required for WebApplicationFactory<T>
    {
        public static void Main(string[] args)
        {
            StartApi.Start(args);
        }

    }
}
