using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public Slider sliderCorridorWidth;
    public TextMeshProUGUI corridorWidthText;

    public Slider sliderMaxRooms;
    public TextMeshProUGUI maxRoomsText;

    public Slider sliderMinSize;
    public TextMeshProUGUI minSizeText;

    public Slider sliderMaxSize;
    public TextMeshProUGUI maxSizeText;

    public TMP_InputField seedText;
    public Toggle personnalSeedCheck;

    public PlayerMovement playerMovement;

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
        rooms = new Room[maxRooms];
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
    }

    void FillScene()
    {
        nbTiles = 0;
        tiles = new GameObject[x * y];
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
                    tiles[nbTiles] = Instantiate(tilePref, place, Quaternion.identity);
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
