namespace BillUtils.SpaceShipData
{
    public enum E_SpaceShipPart
    {
        HEAD,
        WING,
        WEAP,
        ENGINE
    }
    public class SpaceShipHead
    {
        public string Name;
        public E_SpaceShipPart Part;
        public string Path;
    }

    public class SpaceShipWing
    {
        public string Name;
        public E_SpaceShipPart Part;
        public string Path;
    }

    public class SpaceShipWeap
    {
        public string Name;
        public E_SpaceShipPart Part;
        public string Path;
    }

    public class SpaceShipEngine
    {
        public string Name;
        public E_SpaceShipPart Part;
        public string Path;
    }
}