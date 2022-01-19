using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer;

public class DbInitializer : IDbInitializer
{
    public void Initialize()
    {
        // Apply migrations if they are not applied,
        // Create roles if they are not created,
        // If roles were not created, create an admin user as well.
        throw new NotImplementedException();
    }
}
