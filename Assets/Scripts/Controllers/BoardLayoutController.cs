using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardLayoutController : MonoBehaviour
{	
	public float pollingRate, arbitrationSetupTime;
	public GameObject icon;
	public Vector3 presidentEndPos, chancellorEndPos, otherPlayersCenter, yeaStartPos, noStartPos;
	public float playerSpacing;
	public Material topBoardMat, sideBoardMat;
	public Color boardColor, textColor;
	public TextMeshPro ohYeaText, ohNoText;

	private int numYea, numNo = -1;
	public float voteSpacing;

	private Dictionary<int, GameObject> playerObjs = new Dictionary<int, GameObject>();
	private Dictionary<int, Icon> playerIcons = new Dictionary<int, Icon>();
	private Dictionary<int, Vector3> committeePositions = new Dictionary<int, Vector3>();
	private Dictionary<int, Vector3> startingPositions = new Dictionary<int, Vector3>();
	private Dictionary<int, Vector3> votePositions = new Dictionary<int, Vector3>();
	private Dictionary<int, Vector3> currentPositions = new Dictionary<int, Vector3>();

	private List<int> votingIds = new List<int>();


	private float radius = 10;
	private float arbitrationStartTime, votedStartTime, goingBackStartTime;
	private Rect gameRect;
	private Vector3 chancellorStartPos, presidentStartPos;

	private bool isStartingToArbitrate, isMovingVoters, isGoingBack = false;
	private GameObject chancellorObj, presidentObj;
    // Start is called before the first frame update
    void Start () {
		InvokeRepeating("CheckForUpdates", 0f, pollingRate);
		boardColor = topBoardMat.color;
    	boardColor.a = 1;
    	topBoardMat.color = boardColor;
    	sideBoardMat.color = boardColor;
    	textColor = Color.white;
	}

	private void CheckForUpdates(){
		RequestManager.instance.CheckForUpdatesRoomLobby();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
			MakeIconsAroundBoard(RequestManager.instance.currentRoom.users);
        }

        if(Input.GetKeyDown(KeyCode.Z)){
        	SetupArbitration();
        }

		if(Input.GetKeyDown(KeyCode.X)){
        	MoveToNo(209);
        }        

        if(Input.GetKeyDown(KeyCode.C)){
        	MoveToNo(210);
        }

        if(Input.GetKeyDown(KeyCode.A)){
        	GoBackToNormal();
        }
    }

    void FixedUpdate(){
    	if(isStartingToArbitrate){
    		MoveIconsForArbitration();
    	}

    	if(isMovingVoters){
    		MoveIconsThatVoted();
    	}

    	if(isGoingBack){
    		MoveBack();
    	}
    }

    public void MakeIconsAroundBoard(List<RailsUser> users){
    	int count = users.Count;
    	float theta = ((2.0f * Mathf.PI) / count);
		for(int k = 0; k < count; k++){
			float x = Mathf.Clamp(radius * Mathf.Cos(theta*(k)+Mathf.PI/2f), -7.5f, 7.5f);
			float y = Mathf.Clamp(radius * Mathf.Sin(theta*(k)+Mathf.PI/2f), -4f, 4.5f);
			Vector3 newPos = new Vector3(x, y, 0f);
			GameObject go = Instantiate(icon, newPos, Quaternion.identity); 
			Icon newIcon = go.GetComponent<Icon>();
			newIcon.SetAvatar(users[k].avatar);
			newIcon.SetName(users[k].name);
			playerIcons.Add(users[k].id, newIcon);
			playerObjs.Add(users[k].id, go);
		}
    }

    public void MoveIconsForArbitration(){
    	float deltaTime = Time.time - arbitrationStartTime;
    	float t = Mathf.Pow((deltaTime/arbitrationSetupTime), 2f);
    	//move stuff
    	chancellorObj.transform.position = Vector3.Lerp(chancellorStartPos, chancellorEndPos, t);
    	presidentObj.transform.position = Vector3.Lerp(presidentStartPos, presidentEndPos, t);
    	//alpha stuff
    	foreach(KeyValuePair<int, Icon> icon in playerIcons){
    		if(icon.Key == RequestManager.instance.currentRoom.president_id){
    			icon.Value.SetAlphaForTophat(t);
    		}else if(icon.Key == RequestManager.instance.currentRoom.chancellor_id){
    			icon.Value.SetAlphaForPinwheel(t);
    		}else{
    			Vector3 startPos = startingPositions[icon.Key];
    			Vector3 endPos = committeePositions[icon.Key];
    			playerObjs[icon.Key].transform.position = Vector3.Lerp(startPos, endPos, t);
    		}
    	}
    	boardColor = topBoardMat.color;
    	boardColor.a = 1f - t;
    	topBoardMat.color = boardColor;
    	sideBoardMat.color = boardColor;

    	textColor.a = t;
    	ohYeaText.color = textColor;
    	ohNoText.color = textColor;
    	if(t >= 1){
    		//end arbitration
    		isStartingToArbitrate = false;
    	}
    }

    public void MoveBack(){
    	float deltaTime = Time.time - goingBackStartTime;
    	float t = Mathf.Pow((deltaTime/arbitrationSetupTime), 2f);
    	foreach(KeyValuePair<int, Icon> icon in playerIcons){
    		// if(icon.Key == RequestManager.instance.currentRoom.president_id){
    		// 	icon.Value.SetAlphaForTophat(1f - t);
    		// }else if(icon.Key == RequestManager.instance.currentRoom.chancellor_id){
    		// 	icon.Value.SetAlphaForPinwheel(1f - t);
    		// }
			Vector3 startPos = currentPositions[icon.Key];
			Vector3 endPos = startingPositions[icon.Key];
			playerObjs[icon.Key].transform.position = Vector3.Lerp(startPos, endPos, t);
    	}
    	boardColor = topBoardMat.color;
    	boardColor.a = t;
    	topBoardMat.color = boardColor;
    	sideBoardMat.color = boardColor;

    	boardColor = Color.white;
    	boardColor.a = 1f - t;
    	ohYeaText.color = boardColor;
    	ohNoText.color = boardColor;
    	if(t >= 1){
    		//end arbitration
    		isGoingBack = false;
    		committeePositions.Clear();
    		startingPositions.Clear();
    		votePositions.Clear();
    		currentPositions.Clear();
    	}
    }

    public void MoveIconsThatVoted(){
    	float deltaTime = Time.time - votedStartTime;
    	float t = Mathf.Pow((deltaTime/arbitrationSetupTime), 2f);
    	foreach(int id in votingIds){
    		playerObjs[id].transform.position = Vector3.Lerp(committeePositions[id], votePositions[id], t);
    	}
    	if(t >= 1){
    		isMovingVoters = false;
    		votingIds.Clear();
    	}
    }

    public void SetupArbitration(){
    	//will change for prospecitve chancellor
    	chancellorObj = playerObjs[RequestManager.instance.currentRoom.chancellor_id];
    	presidentObj = playerObjs[RequestManager.instance.currentRoom.president_id];
    	chancellorStartPos = chancellorObj.transform.position;
    	presidentStartPos = presidentObj.transform.position;

    	int numCommitteeMembers = RequestManager.instance.currentRoom.CommitteeCount();
    	float unitsLeft = 0.5f*(numCommitteeMembers-1f);
    	Vector3 playerPos = new Vector3(otherPlayersCenter.x - unitsLeft * playerSpacing, otherPlayersCenter.y, otherPlayersCenter.z);
    	foreach(KeyValuePair<int, GameObject> go in playerObjs){
    		//not president or chancellor
    		startingPositions.Add(go.Key, go.Value.transform.position);
    		if(go.Key != RequestManager.instance.currentRoom.president_id && go.Key != RequestManager.instance.currentRoom.chancellor_id){
    			committeePositions.Add(go.Key, playerPos);
    			playerPos.x += playerSpacing;
    		}
    	}

    	arbitrationStartTime = Time.time;
    	isStartingToArbitrate = true;
    }

    public void MoveToYea(int playerId){
    	numYea++;
    	votingIds.Add(playerId);
    	Vector3 endPosition = new Vector3(yeaStartPos.x, yeaStartPos.y - voteSpacing*numYea, yeaStartPos.z);
    	votePositions.Add(playerId, endPosition);
    	votedStartTime = Time.time;
    	isMovingVoters = true;
    }

    public void MoveToNo(int playerId){
    	numNo++;
    	votingIds.Add(playerId);
    	Vector3 endPosition = new Vector3(noStartPos.x, noStartPos.y - voteSpacing*numNo, noStartPos.z);
    	votePositions.Add(playerId, endPosition);
    	votedStartTime = Time.time;
    	isMovingVoters = true;
    }

    public void GoBackToNormal(){
    	foreach(KeyValuePair<int, GameObject> go in playerObjs){
    		//not president or chancellor
    		currentPositions.Add(go.Key, go.Value.transform.position);
    	}
    	numNo = -1;
    	numYea = -1;
    	goingBackStartTime = Time.time;
    	isGoingBack = true;
    }

}