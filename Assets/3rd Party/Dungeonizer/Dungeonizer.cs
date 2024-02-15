using UnityEngine;
using System.Collections.Generic;
using GameEngine;
using System.Linq;

namespace Dungeonizer 
{
	public class Dungeonizer : MonoBehaviour 
	{
		[Tooltip("This prefab will be instantiate on dungeons entrance. You can put your character, or character spawner here.")]
		public GameObject startPrefab;

		[Tooltip("This will be end of level. ")]
		public GameObject exitPrefab;

		public List<SpawnList> spawnedObjectLocations = new List<SpawnList>();
        public SpawnerNode spawnerNodePrefab;
		public GameObject floorPrefab;
		public GameObject wallPrefab;
		public GameObject doorPrefab;
		public GameObject corridorFloorPrefab;
		public GameObject corridorWallPrefab;

		public GameObject cornerPrefab;
		public bool cornerRotation = false;

		public int maximumRoomCount = 10;

		[Tooltip("Min gap between rooms. Also affects corridor lengths ")]
		public int minimumRoomMargin = 0;

		[Tooltip("Maximum gap between rooms. Also affects corridor lengths ")]
		public int roomMargin = 3;
		[Tooltip("If Checked: makes dungeon reset on every time level loads.")]	
		public bool generate_on_load = true;
		public int minRoomSize = 5;
		public int maxRoomSize = 10;
		
		[Tooltip("How big are your tiles? (Affects corridor and room sizes)")]
		public float tileScaling = 1f;
		public List<SpawnOption> spawnOptions = new List<SpawnOption>();
		public List<CustomRoom> customRooms = new List<CustomRoom> ();
		public bool makeIt3d = false;

		public void ClearOldDungeon(bool immediate = false)
		{
			int childs = transform.childCount;
			for (var i = childs - 1; i >= 0; i--)
			{
				if(immediate)
                {
					DestroyImmediate(transform.GetChild(i).gameObject);
				}
				else 
                {
					Destroy(transform.GetChild(i).gameObject);
				}
			}
		}

		public void Generate()
		{
			Dungeon dungeon = new Dungeon ();
			
			dungeon.min_size = minRoomSize;
			dungeon.max_size = maxRoomSize;
			dungeon.maximumRoomCount = maximumRoomCount;
			dungeon.roomMargin = roomMargin;
			dungeon.minimumRoomMargin = minimumRoomMargin;
			
			dungeon.GenerateDungeonData (); //Calculates all static object locations (walls, corridors, doors, corners etc.)
            int totalRooms = Dungeon.rooms.Count;
            List<GameObject> roomParents = new List<GameObject>();

            for(int i = 0; i < totalRooms; i++)
            {
                GameObject roomParent = new GameObject ();
                roomParent.name = "Room " + i.ToString();
                roomParents.Add(roomParent);
                roomParent.transform.parent = transform;
            }

			//after this line, dungeonizer will instantiate objects in their calculated locations.		
			foreach(MapTile mapTile in Dungeon.map) 
            {
				int tile = mapTile.type;
				int orientation = mapTile.orientation;
				GameObject created_tile;
				Vector3 tile_location;
                Room room = mapTile.room;
                if (!makeIt3d) 
				{
					tile_location = new Vector3 (mapTile.x * tileScaling, mapTile.y * tileScaling, 0);
				} 
                else
				{
					tile_location = new Vector3 (mapTile.x * tileScaling, 0, mapTile.y * tileScaling);
				}

				created_tile = null;
				if (tile == 1) 
				{
					GameObject floorPrefabToUse = floorPrefab;
					if(room != null)
					{
						foreach(CustomRoom customroom in customRooms)
						{
							if(customroom.roomId == room.room_id)
							{
								floorPrefabToUse = customroom.floorPrefab;
								break;
							}
						}
					}

					created_tile = GameObject.Instantiate (floorPrefabToUse, tile_location, Quaternion.identity) as GameObject;
				}

				Dungeonizer dungeonizer = this;
				if (Dungeon.walls.Contains(tile) || (Dungeon.corridor_walls.Contains(tile) && !dungeonizer.corridorWallPrefab )) 
				{
					GameObject wallPrefabToUse = wallPrefab;
					if(room != null)
					{
						foreach(CustomRoom customroom in customRooms)
						{
							if(customroom.roomId == room.room_id){
								wallPrefabToUse = customroom.wallPrefab;
								break;
							}
						}
					}


					created_tile = GameObject.Instantiate (wallPrefabToUse, tile_location, Quaternion.identity) as GameObject;                   

					if(!makeIt3d)
					{ //wall rotation stuff here.
						created_tile.transform.Rotate(Vector3.forward  * (90 * (tile -4)));
					}
					else
					{
						created_tile.transform.Rotate(Vector3.up  * (90 * (tile -2))); // 3D corner rotation
					}
				}
				else if(Dungeon.corridor_walls.Contains(tile))
				{
					GameObject wallPrefabToUse = corridorWallPrefab;
					if(room != null)
					{
						foreach(CustomRoom customroom in customRooms)
						{
							if(customroom.roomId == room.room_id){
								wallPrefabToUse = customroom.wallPrefab;
								break;
							}
						}
					}


					created_tile = GameObject.Instantiate (wallPrefabToUse, tile_location, Quaternion.identity) as GameObject;
					if(!makeIt3d)
					{ // wall rotation stuff here.
						created_tile.transform.Rotate(Vector3.forward  * (90 * (tile -4)));
					}
					else
                    {
						created_tile.transform.Rotate(Vector3.up  * (90 * (tile -2)));
					}				
				}
				
				if (tile == 3) 
				{
					if (corridorFloorPrefab)
					{
						created_tile = GameObject.Instantiate(corridorFloorPrefab, tile_location, Quaternion.identity) as GameObject;
					}
					else
					{
						created_tile = GameObject.Instantiate(floorPrefab, tile_location, Quaternion.identity) as GameObject;
					}

					if (orientation == 1 && makeIt3d)
					{
						created_tile.transform.Rotate(Vector3.up * (-90));
					}

				}

				if (Dungeon.corners.Contains(tile)) 
                {
					GameObject cornerPrefabToUse = cornerPrefab;
					if(room != null){
						foreach(CustomRoom customroom in customRooms){
							if(customroom.roomId == room.room_id){
								cornerPrefabToUse = customroom.cornerPrefab;
								break;
							}
						}
					}


					if(cornerPrefabToUse)
                    { //there was a bug in this line. A good man helped for fix. Dungeonizer community is awesome. 			
						created_tile = GameObject.Instantiate (cornerPrefabToUse, tile_location, Quaternion.identity) as GameObject;
						if(cornerRotation)
                        {
							if(!makeIt3d)
                            {
								created_tile.transform.Rotate(Vector3.forward  * (-90 * (tile -4)));
							}
							else
                            {
								created_tile.transform.Rotate(Vector3.up  * (90 * (tile -4)));
							}
						}
					}
					else
                    {
						created_tile = GameObject.Instantiate (wallPrefab, tile_location, Quaternion.identity) as GameObject;
					}
				}
				
				if (created_tile) 
                {
                    if (room != null)
                    {
                        created_tile.transform.parent = roomParents[room.room_id].transform;
                    }
                    else
                    {
                        created_tile.transform.parent = transform;
                    }
                   
				}			
			}

            // Create spawn points
            for(int i = 0; i < roomParents.Count; i++)
            {
                CreateRoomSpawners(roomParents[i].transform);
            }


            GameObject end_point;
			GameObject start_point;
			if (!makeIt3d) 
            {
				end_point = GameObject.Instantiate (exitPrefab, new Vector3 (Dungeon.goalRoom.x * tileScaling, Dungeon.goalRoom.y * tileScaling, 0), Quaternion.identity) as GameObject;
				start_point = GameObject.Instantiate (startPrefab, new Vector3 (Dungeon.startRoom.x * tileScaling, Dungeon.startRoom.y * tileScaling, 0), Quaternion.identity) as GameObject;
				
			} 
            else 
            {
				end_point = GameObject.Instantiate (exitPrefab, new Vector3 ((Dungeon.goalRoom.x + Mathf.FloorToInt(Dungeon.goalRoom.w / 2)) * tileScaling, 0, (Dungeon.goalRoom.y + Mathf.FloorToInt(Dungeon.goalRoom.h / 2)) * tileScaling), Quaternion.identity) as GameObject;
				start_point = GameObject.Instantiate (startPrefab, new Vector3 ((Dungeon.startRoom.x + Mathf.FloorToInt(Dungeon.startRoom.w / 2)) * tileScaling, 0, (Dungeon.startRoom.y + Mathf.FloorToInt(Dungeon.startRoom.h / 2)) * tileScaling), Quaternion.identity) as GameObject;
			}

			
			end_point.transform.parent = transform;
			start_point.transform.parent = transform;
			
			
			//Spawn Objects;
			List<SpawnList> spawnedObjectLocations = new List<SpawnList> ();

			//looks for suitable locations to spawn stuff. (like monsters, chests..)
			foreach(MapTile mapTile in Dungeon.map) 
            {
				if (mapTile.type == 1 
						//do not spawn anything on players start location or finish.
						&& !(mapTile.x == Dungeon.startRoom.x + Mathf.FloorToInt(Dungeon.startRoom.w / 2) && mapTile.y == Dungeon.startRoom.y + Mathf.FloorToInt(Dungeon.startRoom.h / 2))
						&& !(mapTile.x == Dungeon.goalRoom.x + Mathf.FloorToInt(Dungeon.goalRoom.w / 2) && mapTile.y == Dungeon.goalRoom.y + Mathf.FloorToInt(Dungeon.goalRoom.h / 2))
					) {
					var location = new SpawnList ();
					location.byWall = mapTile.isEdge;
					location.wallLocation = mapTile.edgeLocation;
					location.x = mapTile.x;
					location.y = mapTile.y;

					
					if (mapTile.byCorridor) {
						location.byCorridor = true;					
					}
					

					location.room = mapTile.room;

					int roomCenterX = (int)Mathf.Floor(location.room.w / 2) + location.room.x;
					int roomCenterY = (int)Mathf.Floor(location.room.h / 2) + location.room.y;

					if(mapTile.x == roomCenterX + 1 && mapTile.y == roomCenterY + 1 )
					{
						location.inTheMiddle = true;
					}
					spawnedObjectLocations.Add (location);				
				}
				else if (mapTile.type == 3) 
                {
					
					var location = new SpawnList ();
					location.x = mapTile.x;
					location.y = mapTile.y;

					
					if (mapTile.isDoor) 
                    {
						location.byCorridor = true;
						location.asDoor = mapTile.doorDirection;
						location.room = mapTile.room;
						spawnedObjectLocations.Add (location);
					}
					
				}
			}
			
			for (int i = 0; i < spawnedObjectLocations.Count; i++) {
				SpawnList temp = spawnedObjectLocations [i];
				int randomIndex = Random.Range (i, spawnedObjectLocations.Count);
				spawnedObjectLocations [i] = spawnedObjectLocations [randomIndex];
				spawnedObjectLocations [randomIndex] = temp;
			}
			
			int objectCountToSpawn = 0;


			//you will need below 2 lines if you are going to use dynamic pathfinding. And a NavMeshSurface component attached to same gameobject.
			//surface = GetComponent<NavMeshSurface>();
			//surface.BuildNavMesh();




			//Now instantiating gameobjects wanted to "spawn"
			foreach (SpawnOption objectToSpawn in spawnOptions){
				objectCountToSpawn = Random.Range(objectToSpawn.minSpawnCount,objectToSpawn.maxSpawnCount);
				while (objectCountToSpawn > 0){
					bool created = false;

					for (int i = 0;i < spawnedObjectLocations.Count;i++){
						bool createHere= false;
					
						if (!spawnedObjectLocations[i].spawnedObject && !spawnedObjectLocations[i].byCorridor){
							
							if(objectToSpawn.spawnRoom > maximumRoomCount){
								objectToSpawn.spawnRoom = 0;
							}
							if(objectToSpawn.spawnRoom == 0){
								if (objectToSpawn.spawnByWall){
									if (spawnedObjectLocations[i].byWall){
										createHere = true;
									}
								}
								else if (objectToSpawn.spawmInTheMiddle)
								{
									if (spawnedObjectLocations[i].inTheMiddle)
									{
										createHere = true;
									}
								}
								else {
									createHere = true;
								}
							}
							else {
								if (spawnedObjectLocations[i].room.room_id == objectToSpawn.spawnRoom){
									if (objectToSpawn.spawnByWall){
										if (spawnedObjectLocations[i].byWall){
											createHere = true;
										}
									}
									else {
										createHere = true;
									}
								}
							}
						}


						if (createHere){ //means dungeonizer found a suitable place to put object.
							SpawnList spawnLocation = spawnedObjectLocations[i];
							GameObject newObject;
							Quaternion spawnRotation = Quaternion.identity;

							if (!makeIt3d){
								newObject = GameObject.Instantiate(objectToSpawn.gameObject,new Vector3(spawnLocation.x * tileScaling ,spawnLocation.y * tileScaling,0),spawnRotation) as GameObject;
							}
							else {
								if (spawnLocation.byWall)
								{
									if (spawnLocation.wallLocation == "s")
									{
										spawnRotation = Quaternion.Euler(new Vector3(0, 0, 0));
									}
									else if (spawnLocation.wallLocation == "w")
									{
										spawnRotation = Quaternion.Euler(new Vector3(0, 90, 0));
									}
									else if (spawnLocation.wallLocation == "n")
									{
										spawnRotation = Quaternion.Euler(new Vector3(0, 180, 0));
									}
									else if (spawnLocation.wallLocation == "e")
									{
										spawnRotation = Quaternion.Euler(new Vector3(0, 270, 0));
									}

								}
								else
								{
									if (objectToSpawn.spawnRotated)
									{
										spawnRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
									}
									else
									{
										spawnRotation = Quaternion.Euler(new Vector3(0, Random.Range(0,2) * 90, 0));
									}
								}

								newObject = GameObject.Instantiate(objectToSpawn.gameObject,new Vector3(spawnLocation.x * tileScaling ,0 + objectToSpawn.heightFix ,spawnLocation.y * tileScaling),spawnRotation) as GameObject;
							}
							
							newObject.transform.parent = transform;
							spawnedObjectLocations[i].spawnedObject = newObject; 
							objectCountToSpawn--;
							created = true;
							break;
						}
					}
					if (!created) //if cant find anywhere to put, dont put. (prevents endless loops)
					{
						objectCountToSpawn--;
					}
					
				}
			}

			//DOORS
			if (doorPrefab) {
				for (int i = 0; i < spawnedObjectLocations.Count; i++) {
					if (spawnedObjectLocations[i].asDoor > 0){
						GameObject newObject;
						SpawnList spawnLocation = spawnedObjectLocations[i];

						GameObject doorPrefabToUse = doorPrefab;
						Room room = spawnLocation.room;
						if(room != null){
							foreach(CustomRoom customroom in customRooms){
								if(customroom.roomId == room.room_id){
									doorPrefabToUse = customroom.doorPrefab;
									break;
								}
							}
						}

						if (!makeIt3d){
							newObject = GameObject.Instantiate(doorPrefabToUse,new Vector3(spawnLocation.x * tileScaling ,spawnLocation.y * tileScaling,0),Quaternion.identity) as GameObject;
						}
						else {
							newObject = GameObject.Instantiate(doorPrefabToUse,new Vector3(spawnLocation.x * tileScaling ,0,spawnLocation.y * tileScaling),Quaternion.identity) as GameObject;
						}

						if(!makeIt3d){
							newObject.transform.Rotate(Vector3.forward  * (-90 * ( spawnedObjectLocations[i].asDoor - 1)));
						}
						else{
							// 3D DOOR Rotation
							// Append debug info to object name
							newObject.name += " " + spawnedObjectLocations[i].x + " " + spawnedObjectLocations[i].y + " " + spawnedObjectLocations[i].asDoor;
							newObject.transform.Rotate(Vector3.up  * (-90 * ( spawnedObjectLocations[i].asDoor - 1)));
						}

						newObject.transform.parent = transform;
						spawnedObjectLocations[i].spawnedObject = newObject;

					}
				}
			}

		}

		// Use this for initialization
		void Start () 
		{
			if (generate_on_load){
				ClearOldDungeon();
				Generate();
			}
		}

        private void CreateRoomSpawners(Transform t)
        {
            var children = t.gameObject.GetComponentsInChildren<Transform>().ToList();
            children.Remove(t);
            float furthestRight = children[0].position.x;
            float furthestNorth = children[0].position.y;
            float furthestLeft = children[0].position.x;
            float furthestSouth = children[0].position.y;

            Vector3 startVector = children[0].position;
            Debug.Log("Start vector name = " + children[0].gameObject.name);

            foreach (Transform child in children)
            {
                if(child.position.x > furthestRight)
                {
                    furthestRight = child.position.x;
                }
                if(child.position.x < furthestLeft)
                {
                    furthestLeft = child.position.x;
                }
                if (child.position.y > furthestNorth)
                {
                    furthestNorth = child.position.y;
                }
                if (child.position.y < furthestSouth)
                {
                    furthestSouth = child.position.y;
                }
            }

            float width = Mathf.Abs(furthestRight - furthestLeft);
            float height = Mathf.Abs(furthestNorth - furthestSouth);
            int offset = 3;

            
            Vector3 ne = new Vector3(furthestRight - offset, furthestNorth - offset, 0);
            Vector3 se = new Vector3(furthestRight - offset, furthestSouth + offset, 0);
            Vector3 nw = new Vector3(furthestLeft + offset, furthestNorth - offset, 0);
            Vector3 sw = new Vector3(furthestLeft + offset, furthestSouth + offset, 0);
            

            /*
            Vector3 ne = new Vector3(startVector.x + width - offset, startVector.y + height - offset, 0);
            Vector3 se = new Vector3(startVector.x + width - offset, startVector.y + offset, 0);
            Vector3 nw = new Vector3(startVector.x + offset, startVector.y + height - offset, 0);
            Vector3 sw = new Vector3(startVector.x + offset, startVector.y + offset, 0);
            */

            List<Vector3> positions = new List<Vector3> { ne, se, nw, sw};
            positions.ForEach((position) =>
            {
                SpawnerNode newSpawner = Instantiate(spawnerNodePrefab, t);
                newSpawner.transform.localPosition = position;
            });

        }
	}

    public class Dungeon
    {
        public static int map_size;
        public static int map_size_x;
        public static int map_size_y;

        public static List<MapTile> map;
        public static List<Room> rooms = new List<Room>();

        public static Room goalRoom;
        public static Room startRoom;

        public int min_size;
        public int max_size;

        public int maximumRoomCount;
        public int minimumRoomMargin;
        public int roomMargin;
        public int roomMarginTemp;

        //tile types for ease
        public static List<int> roomsandfloors = new List<int> { 1, 3 };
        public static List<int> corners = new List<int> { 4, 5, 6, 7 };
        public static List<int> walls = new List<int> { 8, 9, 10, 11 };
        public static List<int> corridor_walls = new List<int> { 108, 109, 1010, 1011 };
        private static List<string> directions = new List<string> { "x", "y", "-y", "-x" }; //,"-y"};

        public MapTile CreateTile(int type, int x, int y, Room room = null)
        {
            MapTile newRoomTile = new MapTile();
            newRoomTile.type = type;
            newRoomTile.room = room;
            newRoomTile.x = x;
            newRoomTile.y = y;
            if (room != null)
            {
                newRoomTile.z = room.room_height;
                newRoomTile.tile_height = room.room_height;
            }
            return newRoomTile;
        }

        public void GenerateDungeonData()
        {
            int room_count = this.maximumRoomCount;
            int min_size = this.min_size;
            int max_size = this.max_size;
            map = new List<MapTile>();
            rooms = new List<Room>();

            int collision_count = 0;
            string direction = "set";
            string oldDirection = "set";
            Room lastRoom;


            for (var i = 0; i < room_count; i++)
            {
                Room room = new Room();
                if (rooms.Count == 0)
                {
                    //first room
                    room.x = (int)Mathf.Floor(map_size / 2f);
                    room.y = (int)Mathf.Floor(map_size / 2f);

                    room.w = Random.Range(min_size, max_size);
                    if (room.w % 2 == 0) room.w += 1;
                    room.h = Random.Range(min_size, max_size);
                    if (room.h % 2 == 0) room.h += 1;

                    room.branch = 0;
                    lastRoom = room;
                }
                else
                {
                    int branch = 0;
                    if (collision_count == 0)
                    {
                        branch = Random.Range(5, 20); //complexity
                    }
                    room.branch = branch;

                    if (rooms.Count > 1)
                    {
                        lastRoom = rooms[rooms.Count - 1];
                    }

                    else
                    {
                        lastRoom = rooms[0];
                    }
                    int lri = 1;

                    while (lastRoom.dead_end)
                    {
                        lastRoom = rooms[rooms.Count - lri++];
                    }


                    if (direction == "set")
                    {
                        string newRandomDirection = directions[Random.Range(0, directions.Count)];
                        direction = newRandomDirection;
                        while (direction == oldDirection)
                        {
                            newRandomDirection = directions[Random.Range(0, directions.Count)];
                            direction = newRandomDirection;
                        }
                    }
                    this.roomMarginTemp = Random.Range(0, this.roomMargin - 1);

                    room.w = Random.Range(min_size, max_size);
                    if (room.w % 2 == 0) room.w += 1;

                    room.h = Random.Range(min_size, max_size);
                    if (room.h % 2 == 0) room.h += 1;



                    if (direction == "y")
                    {
                        room.x = lastRoom.x + lastRoom.w + this.roomMarginTemp + this.minimumRoomMargin + 2;
                        room.y = lastRoom.y;
                    }
                    else if (direction == "-y")
                    {
                        room.x = lastRoom.x - room.w - this.roomMarginTemp - this.minimumRoomMargin - 2;
                        room.y = lastRoom.y;
                    }
                    else if (direction == "x")
                    {
                        room.y = lastRoom.y + lastRoom.h + this.roomMarginTemp + this.minimumRoomMargin + 2;
                        room.x = lastRoom.x;
                    }
                    else if (direction == "-x")
                    {
                        room.y = lastRoom.y - room.h - this.roomMarginTemp - this.minimumRoomMargin - 2;
                        room.x = lastRoom.x;
                    }
                    room.room_height = roomMarginTemp;

                    room.connectedTo = lastRoom;
                }


                bool doesCollide = this.DoesCollide(room, 0);
                if (doesCollide)
                {
                    i--;
                    collision_count += 1;
                    if (collision_count > 3)
                    {
                        lastRoom.branch = 1;
                        lastRoom.dead_end = true;
                        collision_count = 0;
                    }
                    else
                    {
                        oldDirection = direction;
                        direction = "set";
                    }
                }
                else
                {
                    room.room_id = i;
                    rooms.Add(room);
                    oldDirection = direction;
                    direction = "set";
                    Debug.LogWarning("Room id = " + room.room_id.ToString());
                }
            }

            //room making
            for (int i = 0; i < rooms.Count; i++)
            {
                Room room = rooms[i];
                for (int x = room.x; x < room.x + room.w; x++)
                {
                    for (int y = room.y; y < room.y + room.h; y++)
                    {
                        MapTile newRoomTile = new MapTile();
                        newRoomTile.type = 1;
                        newRoomTile.room = room;
                        newRoomTile.x = x;
                        newRoomTile.y = y;
                        newRoomTile.z = room.room_height;
                        newRoomTile.tile_height = room.room_height;

                        //mark edges:
                        if (y == room.y + room.h - 1) { newRoomTile.isEdge = true; newRoomTile.edgeLocation = "n"; }
                        if (y == room.y) { newRoomTile.isEdge = true; newRoomTile.edgeLocation = "s"; }
                        if (x == room.x) { newRoomTile.isEdge = true; newRoomTile.edgeLocation = "w"; }
                        if (x == room.x + room.w - 1) { newRoomTile.isEdge = true; newRoomTile.edgeLocation = "e"; }

                        //mark corners:
                        if (x == room.x && y == room.y) { map.Add(this.CreateTile(4, room.x - 1, room.y - 1, room)); }
                        if (x == room.x && y == room.y + room.h - 1) { map.Add(this.CreateTile(5, room.x - 1, room.y + room.h, room)); }
                        if (x == room.x + room.w - 1 && y == room.y) { map.Add(this.CreateTile(7, room.x + room.w, room.y - 1, room)); }
                        if (x == room.x + room.w - 1 && y == room.y + room.h - 1) { map.Add(this.CreateTile(6, room.x + room.w, room.y + room.h, room)); }

                        map.Add(newRoomTile);
                    }
                }

                /* these 4 loops creates room walls */
                for (int j = 0; j < room.h; j++)
                {
                    map.Add(CreateTile(11, room.x - 1, room.y + j, room));
                }

                for (int j = 0; j < room.w; j++)
                {
                    map.Add(CreateTile(10, room.x + j, room.y - 1, room));
                }

                for (int j = 0; j < room.h; j++)
                {
                    map.Add(CreateTile(9, room.x + room.w, room.y + j, room));
                }

                for (int j = 0; j < room.w; j++)
                {
                    map.Add(CreateTile(8, room.x + j, room.y + room.h, room));
                }
            }


            //find far far away room
            goalRoom = rooms[rooms.Count - 1];
            //starting point
            startRoom = rooms[0];


            //corridor making
            for (int i = 0; i < rooms.Count; i++)
            {
                Room roomA = rooms[i];
                Room roomB = rooms[i].connectedTo;

                if (roomB != null)
                {
                    var pointA = new Room(); //start
                    var pointB = new Room();
                    bool horizontalCorridor = false;
                    bool nextTileBlocksDoor = false;


                    // Created door count for this corridor
                    int doorCount = 0;
                    int doorDirection = 0;

                    pointA.x = roomA.x + (int)Mathf.Floor(roomA.w / 2); // First Room Center X
                    pointB.x = roomB.x + (int)Mathf.Floor(roomB.w / 2); // Second Room Center X

                    pointA.y = roomA.y + (int)Mathf.Floor(roomA.h / 2); // First Room Center Y
                    pointB.y = roomB.y + (int)Mathf.Floor(roomB.h / 2); // Second Room Center Y

                    if (Mathf.Abs(pointA.x - pointB.x) > Mathf.Abs(pointA.y - pointB.y))
                    {
                        //horizontal
                        horizontalCorridor = true;
                        if (roomA.h > roomB.h)
                        {
                            pointA.y = pointB.y;
                        }
                        else
                        {
                            pointB.y = pointA.y;
                        }
                    }
                    else
                    {
                        //vertical
                        if (roomA.w > roomB.w)
                        {
                            pointA.x = pointB.x;
                        }
                        else
                        {
                            pointB.x = pointA.x;
                        }
                    }

                    MapTile currentTile = null;
                    while ((pointB.x != pointA.x) || (pointB.y != pointA.y))
                    {
                        // So dungeonizer starts from one room's center and goes to other room's center tile by tile.
                        // And it creates a corridor.
                        // This currentDirection means which direction we are going to create a corridor tile.
                        // When its created it doesnt matter if its created left to right or right to left.
                        // But we need to know which direction we are going to create next tile and how to rotate doors etc.

                        int currentDirection = 0;
                        if (horizontalCorridor)
                        {
                            if (pointB.x > pointA.x)
                            {
                                pointB.x--;
                                currentDirection = 4;
                            }
                            else
                            {
                                pointB.x++;
                                currentDirection = 2;
                            }
                        }
                        else
                        {
                            if (pointB.y > pointA.y)
                            {
                                currentDirection = 1;
                                pointB.y--;
                            }
                            else
                            {
                                currentDirection = 3;
                                pointB.y++;
                            }
                        }

                        //This code checks if corridor hits a wall. Also saves it for later to create door if needed.
                        //That means Dungeonizer will try not to spawn anything blocks corridors and doors.					
                        bool isWall = map.Find(item => (item.x == pointB.x && item.y == pointB.y && item.IsWall())) != null;
                        map.RemoveAll(item => (item.x == pointB.x && item.y == pointB.y && item.IsWall()));

                        if (isWall && currentTile != null)
                        {
                            currentTile.byCorridor = true; //this is actually previous tile
                        }

                        //dont spawn anything if there is a floor already
                        currentTile = map.Find(item => (pointB.x == item.x && pointB.y == item.y && item.type == 1));
                        if (currentTile != null && nextTileBlocksDoor)
                        {
                            currentTile.byCorridor = true;
                            nextTileBlocksDoor = false;
                            continue;
                        }
                        else if (currentTile != null)
                        {
                            continue;
                        }

                        MapTile newCorridorTile = new MapTile();
                        newCorridorTile.type = 3;

                        //newCorridorTile.room = room;
                        newCorridorTile.x = pointB.x;
                        newCorridorTile.y = pointB.y;
                        if (isWall)
                        {
                            // if this is the first door:
                            doorCount++;
                            if (doorCount == 1)
                            {
                                doorDirection = currentDirection;
                            }
                            else
                            {
                                doorDirection = currentDirection + 2;
                            }
                            nextTileBlocksDoor = true; //noting this because we want some items to spawn and block corridor entrances.			
                            newCorridorTile.isDoor = true; //this tile could be a door?
                            newCorridorTile.doorDirection = doorDirection;
                            newCorridorTile.debugInfo = " New Corridor Tile " + newCorridorTile.x + " " + newCorridorTile.y + " " + newCorridorTile.doorDirection;

                        }
                        map.Add(newCorridorTile);

                        /* Wider hallways logic
                        for(int j = -2; j < 3; j++)
                        {
                            MapTile newCorridorTile = new MapTile();
                            newCorridorTile.type = 3;

                            int dirBonusX = 0;
                            int dirBonusY = 0;
                            if (horizontalCorridor)
                            {
                                dirBonusY = j;
                            }
                            else
                            {
                                dirBonusX = j;
                            }
                            //newCorridorTile.room = room;
                            newCorridorTile.x = pointB.x + dirBonusX;
                            newCorridorTile.y = pointB.y + dirBonusY;
                            if (isWall)
                            {
                                // if this is the first door:
                                doorCount++;
                                if (doorCount == 1)
                                {
                                    doorDirection = currentDirection;
                                }
                                else
                                {
                                    doorDirection = currentDirection + 2;
                                }
                                nextTileBlocksDoor = true; //noting this because we want some items to spawn and block corridor entrances.			
                                newCorridorTile.isDoor = true; //this tile could be a door?
                                newCorridorTile.doorDirection = doorDirection;
                                newCorridorTile.debugInfo = " New Corridor Tile " + newCorridorTile.x + " " + newCorridorTile.y + " " + newCorridorTile.doorDirection;

                            }
                            map.Add(newCorridorTile);
                        }*/

                        //Corridor wall locations

                        if (horizontalCorridor)
                        {
                            currentTile = map.Find(item => (pointB.x == item.x && pointB.y + 1 == item.y && Dungeon.walls.Contains(item.type)));
                            if (currentTile == null)
                            {
                                map.Add(CreateTile(108, pointB.x, pointB.y + 1));
                            }

                            currentTile = map.Find(item => (pointB.x == item.x && pointB.y - 1 == item.y && Dungeon.walls.Contains(item.type)));
                            if (currentTile == null)
                            {
                                map.Add(CreateTile(1010, pointB.x, pointB.y - 1));
                            }
                        }
                        else
                        {
                            currentTile = map.Find(item => (pointB.x + 1 == item.x && pointB.y == item.y && Dungeon.walls.Contains(item.type)));
                            if (currentTile == null)
                            {
                                map.Add(CreateTile(109, pointB.x + 1, pointB.y));
                            }
                            currentTile = map.Find(item => (pointB.x - 1 == item.x && pointB.y == item.y && Dungeon.walls.Contains(item.type)));
                            if (currentTile == null)
                            {
                                map.Add(CreateTile(1011, pointB.x - 1, pointB.y));
                            }
                        }


                    }

                }
            }

        }

        private bool DoesCollide(Room room, int ignore)
        {
            int random_blankliness = 0;

            for (int i = 0; i < rooms.Count; i++)
            {
                //if (i == ignore) continue;
                var check = rooms[i];
                if (
                    !((room.x + room.w + random_blankliness < check.x) ||
                    (room.x > check.x + check.w + random_blankliness) ||
                    (room.y + room.h + random_blankliness < check.y) ||
                    (room.y > check.y + check.h + random_blankliness)))
                    return true;
            }

            return false;
        }
    }

    public class Room
    {
        public int x = 0;
        public int y = 0;
        public int w = 0;
        public int h = 0;
        public Room connectedTo = null;
        public int branch = 0;
        public string relative_positioning = "x";
        public bool dead_end = false;
        public int room_id = 0;

        public int room_height = 0;
    }

    public class SpawnList
    {
        public int x;
        public int y;
        public bool byWall;
        public string wallLocation;
        public bool inTheMiddle;
        public bool byCorridor;

        public int asDoor = 0;
        public Room room = null;
        public bool spawnedObject;
    }

    [System.Serializable]
    public class SpawnOption
    {
        public int minSpawnCount;
        public int maxSpawnCount;
        public bool spawnByWall;
        public bool spawmInTheMiddle;
        public bool spawnRotated;
        //public bool byCorridor;
        [Tooltip("This is for make spawned object will be higher than ground.")]
        public float heightFix = 0;

        public GameObject gameObject;
        [Tooltip("Use 0 for random room, make sure spawn room isnt bigger than your room count")]
        public int spawnRoom = 0;
    }

    [System.Serializable]
    public class CustomRoom
    {
        [Tooltip("make sure room id isnt bigger than your room count")]
        public int roomId = 1;
        public GameObject floorPrefab;
        public GameObject wallPrefab;
        public GameObject doorPrefab;
        public GameObject cornerPrefab;
    }

    public class MapTile
    {
        public int x = 0;
        public int y = 0;
        public int z = 0;

        public int type = 0; //Default = 0 , Room Floor = 1, Wall = 2, Corridor Floor 3, Room Corners = 4, 5, 6 , 7
        public int orientation = 0;
        public Room room = null;
        public int tile_height = 0;

        public bool isCorner = false;
        public bool isEdge = false;
        public bool isDoor = false;
        public int doorDirection = 0;
        public bool byCorridor = false;
        public string edgeLocation = "";

        public string debugInfo = "";

        public bool IsWall()
        {
            if (this.type >= 8 && this.type <= 11) return true;
            return false;
        }

    }
}