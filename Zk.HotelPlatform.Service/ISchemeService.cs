using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zk.HotelPlatform.Model;

namespace Zk.HotelPlatform.Service
{
    public interface ISchemeService
    {
        bool SaveScheme(Scheme scheme, int userId);
        IEnumerable<Scheme> QuerySchemes();
        Scheme GetScheme(int id);
    }
}
