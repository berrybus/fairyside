using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class NotesMenu : UIScreen {
    public GameObject monsterInfo;
    public GameObject loreInfo;
    public TMP_Text nameText;
    public TMP_Text descText;
    public TMP_Text totalNotes;
    public Image scrollBar;

    private int upMove = 0;
    private int downMove = 0;

    private float startOffsetY = 48f;
    private float startPosY;

    private List<TMP_Text> monsterTexts = new List<TMP_Text>();
    private List<TMP_Text> loreTexts = new List<TMP_Text>();

    private float monsterTextHeight;
    private float loreTextHeight;

    void Awake() {
        startPosY = monsterInfo.transform.localPosition.y;
        playClicks = false;
        CreateNoteLists();
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (GameManager.instance != null) {
            CreateText();
            SetupNoteSection();
            var monstersFound = TotalFound(GameManager.instance.foundMonsterNotes);
            var loreFound = TotalFound(GameManager.instance.foundLoreNotes);
            var total = GameManager.totalLoreNotes + GameManager.totalMonsterNotes;
            totalNotes.text = (monstersFound + loreFound) + "/" + total;
        }
    }

    private void CreateNoteLists() {
        for (int i = 0; i < GameManager.totalMonsterNotes; i++) {
            var name = Instantiate(nameText);
            name.transform.SetParent(monsterInfo.transform);
            monsterTexts.Add(name);
            var desc = Instantiate(descText);
            desc.transform.SetParent(monsterInfo.transform);
            monsterTexts.Add(desc);
        }
        for (int i = 0; i < GameManager.totalLoreNotes; i++) {
            var name = Instantiate(nameText);
            name.transform.SetParent(loreInfo.transform);
            loreTexts.Add(name);
            var desc = Instantiate(descText);
            desc.transform.SetParent(loreInfo.transform);
            loreTexts.Add(desc);
        }
    }

    private int TotalFound(bool[] notes) {
        var found = 0;
        for (int i = 0; i < notes.Length; i++) {
            if (notes[i]) {
                found += 1;
            }
        }
        return found;
    }

    private void CreateText() {
        monsterTextHeight = 0;
        loreTextHeight = 0;

        // print(GameManager.instance.foundMonsterNotes.Length);
        // print(GameManager.totalMonsterNotes);
        // print(NoteRepository.monsterNotes.Length);
        for (int i = 0; i < GameManager.totalMonsterNotes; i++) {
            // print(i);
            var nameEntry = SetupNameEntry(i, monsterTextHeight, monsterTexts);
            nameEntry.text = GameManager.instance.foundMonsterNotes[i] ?  NoteRepository.monsterNotes[i].name : "???";
            monsterTextHeight += 16f;

            var descEntry = SetupDescEntry(i, monsterTextHeight, monsterTexts);
            descEntry.text = GameManager.instance.foundMonsterNotes[i] ? NoteRepository.monsterNotes[i].description : "???";
            monsterTextHeight += GameManager.instance.foundMonsterNotes[i] ? 80f : 16f;

        }

        for (int i = 0; i < GameManager.totalLoreNotes; i++) {
            var nameEntry = SetupNameEntry(i, loreTextHeight, loreTexts);
            nameEntry.text = GameManager.instance.foundLoreNotes[i] ? NoteRepository.loreNotes[i].name : "???";
            loreTextHeight += 16f;

            var descEntry = SetupDescEntry(i, loreTextHeight, loreTexts);
            descEntry.text = GameManager.instance.foundLoreNotes[i] ? NoteRepository.loreNotes[i].description : "???";
            loreTextHeight += GameManager.instance.foundLoreNotes[i] ? 96f : 16f;

        }

    }

    private TMP_Text SetupNameEntry(int index, float itemHeight, List<TMP_Text> list) {
        var nameEntry = list[2 * index];
        nameEntry.transform.localPosition = new Vector3(-34f, -itemHeight + startOffsetY, 1f);
        nameEntry.transform.localScale = Vector3.one;
        return nameEntry;
    }

    private TMP_Text SetupDescEntry(int index, float itemHeight, List<TMP_Text> list) {
        var descEntry = list[2 * index + 1];
        descEntry.transform.localPosition = new Vector3(-34f, -itemHeight + startOffsetY, 1f);
        descEntry.transform.localScale = Vector3.one;
        return descEntry;
    }

    private void SetupNoteSection() {
        monsterInfo.gameObject.SetActive(currentSelect == 1);
        loreInfo.gameObject.SetActive(currentSelect == 0);
    }

    public override void MoveUp(InputAction.CallbackContext ctx) {
        upMove = ctx.ReadValueAsButton() ? 1 : 0;

    }

    public override void MoveDown(InputAction.CallbackContext ctx) {
        downMove = ctx.ReadValueAsButton() ? 1 : 0;
    }

    public override void MoveLeft(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        currentSelect -= 1;
        currentSelect %= options.Length;
        if (currentSelect < 0) {
            currentSelect += options.Length;
        }

        SetTextColors();
        SetupNoteSection();
        GameManager.instance.PlaySFX(manager.moveClip);
    }

    public override void MoveRight(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        currentSelect += 1;
        currentSelect %= options.Length;

        SetTextColors();
        SetupNoteSection();
        GameManager.instance.PlaySFX(manager.moveClip);
    }

    private void Update() {
        int movement = downMove - upMove;
        GameObject info = currentSelect == 1 ? monsterInfo : loreInfo;
        float maxY = currentSelect == 1 ? monsterTextHeight : loreTextHeight;
        maxY += startPosY - 120f;
        maxY = Mathf.Max(startPosY, maxY);
        MoveInfo(movement, info, maxY);
        Vector3 scrollbarPos = scrollBar.transform.localPosition;
        scrollBar.transform.localPosition = new Vector3(scrollbarPos.x, 54 - (info.transform.localPosition.y / maxY) * 120, scrollbarPos.z);
    }

    private void MoveInfo(int movement, GameObject info, float maxY) {
        info.transform.localPosition += new Vector3(0f, 200f * movement * Time.unscaledDeltaTime, 0f);
        var localPos = info.transform.localPosition;
        float clampedY = Mathf.Min(maxY, Mathf.Max(startPosY, localPos.y));
        info.transform.localPosition = new Vector3(localPos.x, clampedY, localPos.z);
    }

    public override void Confirm(InputAction.CallbackContext ctx) {
        if (!ctx.performed) {
            return;
        }
        if (manager) {
            manager.Cancel(ctx);
        } else {
            Cancel(ctx);
        }
    }
}
