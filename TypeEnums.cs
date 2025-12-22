using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stellaris
{
    public enum PlanetType
    {
        Terrestrial,    // 类地行星
        GasGiant,       // 气态巨行星
        IceGiant,       // 冰巨星
        Lava,           // 熔岩行星
        Oceanic,        // 海洋行星
        Desert,         // 沙漠行星
        Ice,            // 冰行星
        Barren,         // 荒芜行星
        Toxic,          // 毒性行星
        GasDwarf        // 气态矮行星
    }
    public enum StarType
    {
        RedDwarf,       // 红矮星
        OrangeDwarf,    // 橙矮星  
        YellowDwarf,    // 黄矮星（类似太阳）
        WhiteDwarf,     // 白矮星
        RedGiant,       // 红巨星
        BlueGiant,      // 蓝巨星
        NeutronStar,    // 中子星
        BinaryStar,     // 双星系统
        Protostar       // 原恒星
    }
}
