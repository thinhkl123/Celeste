using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UICanvas
{
    public TextMeshProUGUI textDialogue;
    [SerializeField] private string[] lineText;
    [SerializeField] private float textSpeed;
    [SerializeField] private Button nextBtn;

    private int idx;
    private string text2;

    private void Start()
    {
        /*
        nextBtn.onClick.AddListener(() =>
        {
            if (text2 == lineText[idx])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                CopyText();
            }
        });
        */
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (text2 == lineText[idx])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                CopyText();
            }
        }
    }

    private void CopyText()
    {
        text2 = string.Empty;
        textDialogue.text = string.Empty;

        foreach (char c in lineText[idx].ToCharArray())
        {
            if (c == '/')
            {
                textDialogue.text += '\n';
            }
            else
            {
                textDialogue.text += c;
            }

            text2 += c;
        }
    }

    public void StartDialogue()
    {
        idx = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        text2 = string.Empty;
        textDialogue.text = string.Empty;

        foreach (char c in lineText[idx].ToCharArray())
        {
            if (c == '/')
            {
                textDialogue.text += '\n';
            }
            else
            {
                textDialogue.text += c;
            }

            text2 += c;

            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        if (idx < lineText.Length-1)
        {
            idx++;
            StartCoroutine(TypeLine());
        }
        else
        {
            Close(0);
            GameManager.Ins.ChangeToNewPlaying();
            GameManager.Ins.state = GameManager.GameState.Playing;
        }
    }
}
