using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris
{
    public interface IUniversable
    {
        StarSystem StarSystem { get; set; }
        Planet Planet { get; set; }
        int DrawPrority { get; set; }
        void DrawOnGalaxyGUI();
    }
}
