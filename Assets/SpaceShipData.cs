using System.Collections.Generic;
using UnityEngine;
using BillUtils.SpaceShipData;

[CreateAssetMenu(fileName = "SpaceShipData", menuName = "SpaceShipData", order = 0)]
public class SpaceShipData : ScriptableObject
{
    public List<SpaceShipHead> spaceShipHeads;
    public List<SpaceShipWing> spaceShipWings;
    public List<SpaceShipWeap> spaceShipWeaps;
    public List<SpaceShipEngine> spaceShipEngines;
}
