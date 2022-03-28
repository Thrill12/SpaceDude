using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

public class DialogueParser : MonoBehaviour
{ 
    public DialogueGraph graph;
    Coroutine _parser;
    public bool isPlaying = false;

    private UIManager uiManager;
    private QuestManager questManager;
    private NPCManager npcManager;

    private void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
        npcManager = GameObject.FindGameObjectWithTag("NPCManager").GetComponent<NPCManager>();
    }

    public void StartDialogueGraph()
    {
        if (!isPlaying)
        {
            uiManager.playerInput.SwitchCurrentActionMap("UI");
            isPlaying = true;
            //Finding the first node in the graph, as marked by the start node
            foreach (BaseNode item in graph.nodes)
            {
                if (item.GetString() == "Start")
                {
                    graph.current = item;
                    break;
                }
            }

            _parser = StartCoroutine(ParseNode());
        }        
    }

    IEnumerator ParseNode()
    {
        BaseNode b = graph.current;
        
        string data = b.GetString();

        string[] parts = {};

        try
        {
            parts = data.Split("/");
        }
        catch
        {
            Debug.Log("Data couldnt be parsed - data here: " + data);
        }
        

        if (parts[0].Trim() == "Start")
        {           
            NextNode("exit");
        }
        else if (parts[0].Trim() == "Dialogue")
        {
            uiManager.playerInput.SwitchCurrentActionMap("Dialogue");
            uiManager.DisplayDialogue(parts[1], GetComponent<NPC>());

            yield return new WaitForSeconds(parts[1].Length * uiManager.letterTypingPause);

            NextNode("exit");
        }
        else if (parts[0].Trim() == "Quest")
        {
            QuestNode q = (QuestNode)b;

            questManager.AddQuest(q.GetQuest(), GetComponent<NPC>());          

            NextNode("exit");
        }
        else if(parts[0].Trim() == "TwoChoice")
        {
            //Index 1 is the dialogue for the main textbox, 2 is the pass button string, 3 is the fail button string
            uiManager.playerInput.SwitchCurrentActionMap("Dialogue");
            uiManager.DisplayDialogue(parts[1], GetComponent<NPC>());

            yield return new WaitForSeconds(parts[1].Length * uiManager.letterTypingPause);

            uiManager.DisplayChoices(new List<string> { parts[2], parts[3] });

            yield return new WaitUntil(() => uiManager.hasChosenDialogueChoice);

            if(uiManager.chosenDialogueChoice == 0)
            {
                //Pass
                NextNode("pass");
            }
            else
            {
                //Fail
                NextNode("fail");
            }

            uiManager.hasChosenDialogueChoice = false;            
        }
        else if(parts[0].Trim() == "QuestCheckRequirements")
        {
            CheckQuestRequirementsNode node = (CheckQuestRequirementsNode)graph.current;
            if (node.GetQuest().CheckRequirements())
            {
                NextNode("pass");
            }
            else
            {
                NextNode("fail");
            }
        }
        else if(parts[0].Trim() == "QuestCheckCompletion")
        {
            int id = Convert.ToInt32(parts[1].Trim());
            Debug.Log(id + " is id");
            if (questManager.IsCompleted(id))
            {
                NextNode("completed");
                Debug.Log("Completed");
            }
            else
            {
                NextNode("notcompleted");
                Debug.Log("Not completed");
            }
        }
        else if(parts[0].Trim() == "GraphChange")
        {
            ChangeToGraphNode node = graph.current as ChangeToGraphNode;
            npcManager.SetNPCToGraph(GetComponent<NPC>().npcName, node.graphToChangeTo);

            isPlaying = false;
            //If we start the new graph immediately
            if (parts[1].Trim() == "True")
            {
                StartDialogueGraph();
            }
            else
            {
                uiManager.playerInput.SwitchCurrentActionMap("PlayerSuit");
            }
            
        }
        else if (parts[0].Trim() == "SetNPCToGraph")
        {
            SetNPCToGraphNode node = graph.current as SetNPCToGraphNode;
            npcManager.SetNPCToGraph(node.npcToChange, node.graphToChangeTo);            
            NextNode("exit");
        }
    }

    public void NextNode(string fieldName)
    {      
        if (_parser != null)
        {
            StopCoroutine(_parser);
            _parser = null;
        }
      
        //Looks for the first node with the  name fieldname and sets that node as current node to move to it
        foreach (NodePort p in graph.current.Ports)
        {
            if (p.fieldName == fieldName.Trim())
            {
                if(p.ConnectionCount > 0)
                {
                    graph.current = p.Connection.node as BaseNode;
                }
                else
                {
                    graph.current = null;
                }

                break;             
            }
        }

        if(graph.current != null)
        {
            _parser = StartCoroutine(ParseNode());
        }
        else
        {
            isPlaying = false;
            uiManager.playerInput.SwitchCurrentActionMap("PlayerSuit");
        }
    }
}
