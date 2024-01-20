using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RoomManager : MonoBehaviour
{
    public int x, y, minSize, maxSize;
    public int maxRooms;
    public Room[] rooms;
    public int nbRooms = 0;
    public GameObject roomPref;
    public GameObject tilePref;
    public float CorridorWidth = 1f;
    GameObject[] tiles;
    public int seed;
    bool personnalSeed;
    public float distanceChairTable = 0.5f;
    public float distanceWallTorchX = 0.1f;
    public float distanceWallTorchZ = 0.1f;
    public float torchHigh = 0.5f;

    public UnityEngine.UI.Slider sliderCorridorWidth;
    public TextMeshProUGUI corridorWidthText;

    public UnityEngine.UI.Slider sliderMaxRooms;
    public TextMeshProUGUI maxRoomsText;

    public UnityEngine.UI.Slider sliderMinSize;
    public TextMeshProUGUI minSizeText;

    public UnityEngine.UI.Slider sliderMaxSize;
    public TextMeshProUGUI maxSizeText;

    public TMP_InputField seedText;
    public UnityEngine.UI.Toggle personnalSeedCheck;

    public PlayerMovement playerMovement;

    public float propsDensity;
    public GameObject[] propsPref;
    public GameObject[] columnsPref;
    public GameObject[] rocksPref;
    public GameObject chestPref;
    public GameObject tablePref;
    public GameObject chairPref;
    public GameObject torchPref;
    public GameObject[] props;
    private int nbProps = 0;

    public TorchManager torchManager;

    int nbTiles;
    void Awake()
    {
        sliderCorridorWidth.onValueChanged.AddListener((v) =>
        {
            corridorWidthText.text = v.ToString("0.00");
            CorridorWidth = v;
        });

        sliderMaxRooms.onValueChanged.AddListener((v) =>
        {
            maxRoomsText.text = v.ToString("000");
            maxRooms = (int) v;
        });

        sliderMinSize.onValueChanged.AddListener((v) =>
        {
            minSizeText.text = v.ToString("00");
            minSize = (int) v;
        });

        sliderMaxSize.onValueChanged.AddListener((v) =>
        {
            maxSizeText.text = v.ToString("00");
            maxSize = (int) v;
        });

        personnalSeed = false;
        seedText.interactable = false;
        seedText.characterLimit = 9;

        props = new GameObject[10000];
        Init();
    }

    private void GenerateSeed()
    {
        if (!personnalSeed)
        {
            seed = Random.Range(0, 999999999);
            Random.seed = seed;
            seedText.text = seed.ToString();
        }
        else
        {
            Random.seed = System.Convert.ToInt32(seedText.text);
        }
    }

    public void Init()
    {
        DestroyScene();
        GenerateSeed();
        nbRooms = 0;
        nbProps = 0;
        rooms = new Room[maxRooms];
        torchManager.GenerateTorchColor();
        for (int i = 0; i < maxRooms; i++)
        {
            rooms[nbRooms] = Instantiate(roomPref, Vector3.zero, Quaternion.identity).GetComponent<Room>();
            rooms[nbRooms].CreateRoom(minSize, maxSize, x, y);

            if (!IsTouchingOtherRoom(rooms[nbRooms]))
            {
                if (nbRooms == 0)
                {
                    playerMovement.playerPos = rooms[0].GetCenter();
                }
                nbRooms++;
            }
            else
            {
                Destroy(rooms[nbRooms].gameObject);
            }
        }
        FindNearestRooms();
        FillScene();
        CreateCorridors();
        PlaceProps();
        PlaceChest();
    }

    private void PlaceProps()
    {
        int nbPoints = (int) Mathf.Ceil(nbRooms*propsDensity);
        int roomIndex, nbPropsToPlace;
        Vector3 roomPos, roomSize;
        for (int i=0; i<nbPoints; i++)
        {
            roomIndex = i;

            roomPos = rooms[roomIndex].GetPos();
            roomSize = rooms[roomIndex].GetSize();

            PlaceRocks(roomPos, roomSize);
            PlaceColumns(roomPos, roomSize);
            PlaceTorches(roomPos, roomSize);
            if (Random.Range(0,4) == 0)
            {
                PlaceTables(roomPos, roomSize);
            }

            nbPropsToPlace = Random.Range(3, (int) roomSize.x); // faire en fonction de la taille de la salle
            Vector3 propsPos = new Vector3(roomPos.x + Random.Range(roomSize.x / 4, roomSize.x- roomSize.x / 4), 1f, roomPos.z + Random.Range(roomSize.z / 4, roomSize.z- roomSize.z / 4));

            for (int j=0; j<nbPropsToPlace; j++)
            {
                Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                Vector3 changePos = new Vector3(Random.Range(-1f*roomSize.x/4, 1f * roomSize.x / 4), 0, Random.Range(-1f * roomSize.z / 4, 1f * roomSize.z / 4));
                props[nbProps] = Instantiate(propsPref[Random.Range(0, propsPref.Length)], propsPos + changePos, randomRotation);
                nbProps++;
            }
        }
    }

    private void PlaceRocks(Vector3 roomPos, Vector3 roomSize)
    {
        int nbRocks = Random.Range(5, (int) roomSize.x*2);
        for (int i=0; i<nbRocks;i++)
        {
            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Vector3 rocksPos = new Vector3(roomPos.x + Random.Range(0, roomSize.x-1), -0.1f, roomPos.z + Random.Range(0, roomSize.z-1));
            props[nbProps] = Instantiate(rocksPref[Random.Range(0, rocksPref.Length)], rocksPos, randomRotation);
            nbProps++;
        }
    }

    private void PlaceColumns(Vector3 roomPos, Vector3 roomSize)
    {
        int nbColumns = Random.Range(0, 5);
        Vector3[] columnsPos = { new(roomPos.x+.53f, 1f, roomPos.z+.53f), new(roomPos.x+.53f, 1f, roomPos.z + roomSize.z-1.53f),
            new(roomPos.x + roomSize.x-1.53f, 1f, roomPos.z+.53f), new(roomPos.x + roomSize.x - 1.53f, 1f, roomPos.z + roomSize.z-1.53f)};
        for (int i = 0; i < nbColumns; i++)
        {
            Vector3 columnPos = columnsPos[i];
            props[nbProps] = Instantiate(columnsPref[Random.Range(0, columnsPref.Length)], columnPos, Quaternion.identity);
            nbProps++;
        }
    }

    void PlaceTorches(Vector3 roomPos, Vector3 roomSize)
    {
        int nbTorchX = (int) roomSize.x / 5;
        int nbTorchZ = (int)roomSize.z / 5;

        for (int i=0; i<nbTorchX; i++)
        {
            Quaternion rot = Quaternion.Euler(0f, 0f, 0f);
            Vector3 pos = new Vector3(roomPos.x + (roomSize.x / (nbTorchX + 1)) * (i + 1), torchHigh, roomPos.z + distanceWallTorchZ);
            props[nbProps] = Instantiate(torchPref, pos, rot);
            nbProps++;
            rot = Quaternion.Euler(0f, 180f, 0f);
            pos = new Vector3(roomPos.x + (roomSize.x / (nbTorchX + 1)) * (i + 1), torchHigh, roomPos.z + distanceWallTorchX + roomSize.z - 1.53f);
            props[nbProps] = Instantiate(torchPref, pos, rot);
            nbProps++;
        }

        for (int i = 0; i < nbTorchZ; i++)
        {
            Quaternion rot = Quaternion.Euler(0f, 90f, 0f);
            Vector3 pos = new Vector3(roomPos.x + distanceWallTorchZ, torchHigh, roomPos.z + (roomSize.z / (nbTorchZ + 1)) * (i + 1));
            props[nbProps] = Instantiate(torchPref, pos, rot);
            nbProps++;
            rot = Quaternion.Euler(0f, 270f, 0f);
            pos = new Vector3(roomPos.x + distanceWallTorchX + roomSize.x - 1.53f, torchHigh, roomPos.z + (roomSize.z / (nbTorchZ + 1)) * (i + 1));
            props[nbProps] = Instantiate(torchPref, pos, rot);
            nbProps++;
        }
    }

    void PlaceTables(Vector3 roomPos, Vector3 roomSize)
    {
        Vector3 tablePos = new Vector3(roomPos.x + Random.Range(0, roomSize.x - roomSize.x / 4), 1f, roomPos.z + Random.Range(0, roomSize.z - roomSize.z / 4));
        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        props[nbProps] = Instantiate(tablePref, tablePos, randomRotation);
        nbProps++;

        // place chairs
        Vector3[] chairPos = { tablePos + new Vector3(distanceChairTable, 0, distanceChairTable), tablePos + new Vector3(distanceChairTable, 0, -distanceChairTable), 
            tablePos + new Vector3(-distanceChairTable, 0, distanceChairTable), tablePos + new Vector3(-distanceChairTable, 0, -distanceChairTable) };
        int nbChairs = Random.Range(1, 5);
        for (int i = 0; i < nbChairs; i++)
        {
            randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            props[nbProps] = Instantiate(chairPref, chairPos[i], randomRotation);
            nbProps++;
        }
    }

    private void PlaceChest()
    {
        int chestRoom = Random.Range(1, nbRooms);
        props[nbProps] = Instantiate(chestPref, rooms[chestRoom].GetCenter() + new Vector3(0,2,0), Quaternion.identity);
        nbProps++;
    }


    public void CheckTorch()
    {
        for (int i=0; i<nbProps; i++)
        {
            if (props[i]!=null && props[i].CompareTag("Torch"))
            {
                props[i].GetComponent<DetectWall>().CheckWall();
            }
        }
    }

    private void Update()
    {
        personnalSeed = personnalSeedCheck.isOn;
        seedText.interactable = personnalSeed;
    }

    bool IsTouchingOtherRoom(Room room)
    {
        Vector3 size = room.GetSize();
        Vector3 pos = room.GetPos();
        for (int i= 0; i<nbRooms; i++)
        {
            Vector3 size_i = rooms[i].GetSize() + new Vector3(4, 0, 4);
            Vector3 pos_i = rooms[i].GetPos() + new Vector3(-2, 0, -2);

            if (CheckCollision(pos_i, pos, size_i, size))
            {
                return true;
            }
        }
        return false;
    }

    bool CheckCollision(Vector3 a, Vector3 b, Vector3 size_a, Vector3 size_b)
    {
        Vector3 a2 = new Vector3(a.x + size_a.x, 0, a.z + size_a.z);
        Vector3 b2 = new Vector3(b.x + size_b.x, 0, b.z + size_b.z);

        return a.x < b2.x && a2.x > b.x && a.z < b2.z && a2.z > b.z;
    }

    void FindNearestRooms()
    {
        foreach (Room room in rooms)
        {
            if (room != null)
            {
                ConnectRoom(room);
            }
        }
    }

    void ConnectRoom(Room room)
    {
        Vector3 pos = room.GetCenter();
        float minDistance = float.MaxValue;
        float d;
        foreach (Room other in rooms)
        {
            if (other != room && other != null)
            {
                d = Distance(pos, other.GetCenter());
                if (d <= minDistance && (other.nearestRoom == null || !RoomConnected(room, other)))
                {
                    room.nearestRoom = other;
                    room.distanceNearestRoom = d;
                    minDistance = d;
                }
            }
        }
    }


    // algo récursif de parcours des nearestRoom pour ne pas créer de boucles : permet de rendre toutes les salles du donjons accessibles depuis n'importe quelle salle 
    bool RoomConnected(Room r1, Room r2)
    {
        if (r2.nearestRoom == null)
        {
            return false;
        } else if (r2.nearestRoom == r1)
        {
            return true;
        } else
        {
            return RoomConnected(r1, r2.nearestRoom);
        }
    }

    float Distance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
    }

    void DestroyScene()
    {
        for (int i=0; i<nbTiles; i++)
        {
            Destroy(tiles[i]);
        }

        for (int i = 0; i < nbProps; i++)
        {
            Destroy(props[i]);
        }
    }

    void FillScene()
    {
        nbTiles = 0;
        tiles = new GameObject[x * y];
        Quaternion[] tileRot = { Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f), Quaternion.Euler(0f, 180f, 0f), Quaternion.Euler(0f, 270f, 0f) };
        for (int i=0; i<x; i++)
        {
            for (int j=0; j<y; j++)
            {
                bool freePlace = true;
                Vector3 place = new Vector3(i, 0.5f, j);
                Vector3 placeSize = new Vector3(1f, 1f, 1f);
                foreach (Room room in rooms)
                {
                    if (room != null)
                    {
                        if (CheckCollision(place, room.GetPos(), placeSize, room.GetSize()))
                        {
                            freePlace = false;
                        }
                    }
                }
                if (freePlace)
                {
                    int rot = Random.Range(0, 4);
                    tiles[nbTiles] = Instantiate(tilePref, place, tileRot[rot]);
                    nbTiles++;
                }
            }
        }
    }

    void CreateCorridors()
    {
        foreach (Room room in rooms)
        {
            if (room != null)
            {
                Room near = room.nearestRoom;
                if (near != null)
                {
                    CreateCorridor(room, near);
                }
            }
        }
    }

    void CreateCorridor(Room room1, Room room2)
    {
        Vector3 dir = (room2.transform.position - room1.transform.position).normalized;
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(room1.transform.position, CorridorWidth, dir, room1.distanceNearestRoom);
        if (hits.Length > 0) 
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    Destroy(hit.transform.gameObject);
                }
            }
        }
    }
}
