using System;

namespace BillUtils.SpaceShipData
{
    [Serializable]
    public enum E_SpaceShipPart
    {
        NONE,
        HEAD,
        WING,
        WEAP,
        ENGINE
    }
    public class SpaceShipPartBase
    {
        public string Name;
        public E_SpaceShipPart Part;
        public string Path;
    }
    [Serializable]
    public class SpaceShipHead : SpaceShipPartBase { }

    [Serializable]
    public class SpaceShipWing : SpaceShipPartBase { }
    [Serializable]
    public class SpaceShipWeap : SpaceShipPartBase { }

    [Serializable]
    public class SpaceShipEngine : SpaceShipPartBase { }
}