using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Interfaces
{
    public interface IPubSubRepository
    {
          Task<string> Publish(File file);
    }
}
