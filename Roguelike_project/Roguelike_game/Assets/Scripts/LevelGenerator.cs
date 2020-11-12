using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public GameObject layoutRoom;
    public Color startColor, endColor, shopColor, treasureColor;

    public int distanceToEnd;
    public bool includeShop, includeTreasure;
    public int minDistanceToShop, maxDistanceToShop;
    public int minDistanceToTreasure, maxDistanceToTreasure;

    public Transform generatorPoint;

    public enum Direction
    {
        up,
        right,
        down,
        left
    };

    public Direction selectedDirection;

    public float xOffset = 18f, yOffset = 10f;

    public LayerMask whatIsRoom;

    private GameObject endRoom, shopRoom, treasureRoom;

    private List<GameObject> layoutRoomObjects = new List<GameObject>();

    public RoomPrefabs rooms;
    
    private List<GameObject> generatedOutlines = new List<GameObject>();

    public RoomCenter centerStart, centerEnd, centerShop, centerTreasure;

    public RoomCenter[] potentialCenters;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(layoutRoom, generatorPoint.position, generatorPoint.rotation).GetComponent<SpriteRenderer>().color = startColor;

        selectedDirection = (Direction)Random.Range(0, 4);
        movePoint();

        for (int i = 0; i < distanceToEnd; i++)
        {
            GameObject newRoom = Instantiate(layoutRoom, generatorPoint.position, generatorPoint.rotation);
            
            layoutRoomObjects.Add(newRoom);

            if (i + 1 == distanceToEnd)
            {
                newRoom.GetComponent<SpriteRenderer>().color = endColor;
                layoutRoomObjects.RemoveAt(layoutRoomObjects.Count - 1);

                endRoom = newRoom;
            }

            selectedDirection = (Direction) Random.Range(0, 4);
            movePoint();

            while (Physics2D.OverlapCircle(generatorPoint.position, .2f, whatIsRoom))
            {
                movePoint();
            }
        }

        if (includeShop)
        {
            int shopSelector = Random.Range(minDistanceToShop, maxDistanceToShop + 1);

            shopRoom = layoutRoomObjects[shopSelector];
            layoutRoomObjects.RemoveAt(shopSelector);
            shopRoom.GetComponent<SpriteRenderer>().color = shopColor;
        }
        
        if (includeTreasure)
        {
            int treasureSelector = Random.Range(minDistanceToTreasure, maxDistanceToTreasure);

            treasureRoom = layoutRoomObjects[treasureSelector];
            layoutRoomObjects.RemoveAt(treasureSelector);
            treasureRoom.GetComponent<SpriteRenderer>().color = treasureColor;
        }
        
        // create room outlines
        CreateRoomOutline(Vector3.zero);
        foreach (GameObject room in layoutRoomObjects)
        {
            CreateRoomOutline(room.transform.position);
        }
        
        CreateRoomOutline(endRoom.transform.position);

        if (includeShop)
        {
            CreateRoomOutline(shopRoom.transform.position);
        }

        if (includeTreasure)
        {
            CreateRoomOutline(treasureRoom.transform.position);
        }

        foreach (GameObject outline in generatedOutlines)
        {
            bool generateCenter = true;
            
            if (outline.transform.position == Vector3.zero)
            {
                Instantiate(centerStart, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();

                generateCenter = false;
            }

            if (outline.transform.position == endRoom.transform.position)
            {
                Instantiate(centerEnd, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();

                generateCenter = false;

            }

            if (includeShop)
            {
                if (outline.transform.position == shopRoom.transform.position)
                {
                    Instantiate(centerShop, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();

                    generateCenter = false;

                }
            }

            if (includeTreasure)
            {
                if (outline.transform.position == treasureRoom.transform.position)
                {
                    Instantiate(centerTreasure, outline.transform.position, transform.rotation).theRoom =
                        outline.GetComponent<Room>();

                    generateCenter = false;
                }
            }
            
            if (generateCenter)
            {
                int centerSelect = Random.Range(0, potentialCenters.Length);

                Instantiate(potentialCenters[centerSelect], outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void movePoint()
    {
        switch (selectedDirection)
        {
            case Direction.up:
                generatorPoint.position += new Vector3(0f, yOffset, 0f);
                break;
            
            case Direction.down:
                generatorPoint.position += new Vector3(0f, -yOffset, 0f);
                break;
            
            case Direction.right:
                generatorPoint.position += new Vector3(xOffset, 0f, 0f);
                break;
            
            case Direction.left:
                generatorPoint.position += new Vector3(-xOffset, 0f, 0f);
                break;
        }
    }

    public void CreateRoomOutline(Vector3 roomPosition)
    {
        bool roomAbove = Physics2D.OverlapCircle(roomPosition + new Vector3(0f, yOffset, 0f), .2f, whatIsRoom);
        bool roomBelow = Physics2D.OverlapCircle(roomPosition + new Vector3(0f, -yOffset, 0f), .2f, whatIsRoom);
        bool roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0f, 0f), .2f, whatIsRoom);
        bool roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0f, 0f), .2f, whatIsRoom);

        int directionCount = 0;
        if (roomAbove)
        {
            directionCount++;
        }

        if (roomBelow)
        {
            directionCount++;
        }
        
        if (roomRight)
        {
            directionCount++;
        }
        
        if (roomLeft)
        {
            directionCount++;
        }

        switch (directionCount)
        {
            case 0:
                Debug.LogError("Found no room exists!");
                break;
            
            case 1:

                if (roomAbove)
                {
                   generatedOutlines.Add(Instantiate(rooms.singleUp, roomPosition, transform.rotation));
                }
                
                if (roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleDown, roomPosition, transform.rotation));
                }
                
                if (roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleRight, roomPosition, transform.rotation));
                }
                
                if (roomLeft)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleLeft, roomPosition, transform.rotation));
                }
                
                break;
            
            case 2:

                if (roomAbove && roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleUpDown, roomPosition, transform.rotation));
                }
                
                if (roomLeft && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleLeftRight, roomPosition, transform.rotation));
                }
                
                if (roomAbove && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleUpRight, roomPosition, transform.rotation));
                }
                
                if (roomRight && roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleDownRight, roomPosition, transform.rotation));
                }
                
                if (roomBelow && roomLeft)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleDownLeft, roomPosition, transform.rotation));
                }

                if (roomLeft && roomAbove)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleUpLeft, roomPosition, transform.rotation));
                }

                break;
            
            case 3:

                if (roomAbove && roomRight && roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleUpDownRight, roomPosition, transform.rotation));
                }
                
                if (roomLeft && roomRight && roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleDownLeftRight, roomPosition, transform.rotation));
                }
                
                if (roomAbove && roomLeft && roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleUpDownLeft, roomPosition, transform.rotation));
                }
                
                if (roomLeft && roomAbove && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleUpLeftRight, roomPosition, transform.rotation));
                }

                break;
            
            case 4:

                if (roomAbove && roomBelow && roomLeft && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.fourway, roomPosition, transform.rotation));
                }

                break;
        }
    }
}

[System.Serializable]
public class RoomPrefabs
{
    public GameObject singleUp, singleDown, singleLeft, singleRight, 
        doubleUpDown, doubleLeftRight, doubleUpRight, doubleDownRight, 
        doubleDownLeft, doubleUpLeft, tripleUpDownRight, tripleDownLeftRight,
        tripleUpDownLeft, tripleUpLeftRight, fourway;
}