using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

public class NodeParse : MonoBehaviour
{
    public DialogueGraph graph;
    Coroutine _parser;

    public void StartDialogueGraph()
    {
        //Finding the first node in the graph, as marked by the start node
        foreach (BaseNode item in graph.nodes)
        {
            if(item.GetString() == "Start")
            {
                graph.current = item;
                break;
            }
        }

        _parser = StartCoroutine(ParseNode());
    }

    IEnumerator ParseNode()
    {
        BaseNode b = graph.current;

        string data = b.GetString();

        string[] parts = data.Split("/");

        if (parts[0].Trim() == "Start")
        {
            NextNode("exit");
            Debug.Log("Triggered start");
        }
        else if (parts[0].Trim() == "Dialogue")
        {
            Debug.Log("Triggered dialogue");

            //Bool that is activated when the player presses a key
            UIManager.instance.triggeredNextStep = false;

            UIManager.instance.triggeredNextStep = false;
            UIManager.instance.DisplayDialogue(parts[1], GetComponent<NPC>().npcName);

            yield return new WaitForSeconds(parts[1].Length * UIManager.instance.letterTypingPause);

            NextNode("exit");
        }
        else if (parts[0].Trim() == "Quest")
        {
            QuestNode q = (QuestNode)b;

            QuestManager.instance.AddQuest(q.GetQuest());

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

        foreach (NodePort p in graph.current.Ports)
        {
            if (p.fieldName == fieldName.Trim())
            {
                graph.current = null;
                graph.current = p.Connection.node as BaseNode;

                break;
            }
            else
            {
                graph.current = null;
            }
        }

        if(graph.current != null)
        {
            _parser = StartCoroutine(ParseNode());
        }      
    }
}
