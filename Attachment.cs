using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachment : MonoBehaviour {

    public enum AttachmentType {
        Bulldozer,
        Shovel,
        Trencher,    
    }


    private Player playerScript;

    private SpriteRenderer spriteRenderer;
    private GameObject playerObj;        // プレイヤーオブジェクト(プレイヤーに持たれる前提)
    private string[] colliderTags;    // 攻撃可能な敵のタグ(敵接触判定用タグ) 
    private bool isRightFront = true;


    // 以下プランナ、デバッグ用設定フィールド
    [SerializeField, TooltipAttribute("アタッチメントの座標")] private Vector2 attachMentPos;        // アタッチメント用座標情報(プレイヤーとの相対座標)   
    [SerializeField, TooltipAttribute("Bulldozerのoffset")] private Vector2 offsetBulldozer;       // Bulldozerのoffset
    [SerializeField, TooltipAttribute("Shovelのoffset")] private Vector2 offsetShovel;             // Shovelのoffset    
    [SerializeField, TooltipAttribute("Trencherのoffset")] private Vector2 offsetTrencher;         // Trencherのoffset

    // Start is called before the first frame update
    void Start() {
        // spriteRenderer
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        if(this.spriteRenderer == null){
            Debug.LogError("SpriteRendererが設定されていません");
            return;
        }
        
        this.playerScript = playerObj.GetComponent<Player>();
        if(this.playerScript == null){
            Debug.LogError("player Scriptが設定されていません");
            return;
        }

    }

    // Update is called once per frame
    void Update() {
        if(this.playerObj == null){
            return;
        }

        Vector2 offset = Vector2.zero;
        switch(this.playerScript.equipmentAttachment){
            case AttachmentType.Bulldozer:
                offset = this.offsetBulldozer;
                break;
            case AttachmentType.Shovel:
                offset = this.offsetShovel;
                break;
            case AttachmentType.Trencher:
                offset = this.offsetTrencher;
                break;
            default:
                offset = Vector2.zero;
                break;
        }

        // プレイヤーに追従(相対座標で動く)
        this.gameObject.transform.position = this.attachMentPos + offset;

        // 向き(画像)の設定
        this.spriteRenderer.flipX = !this.isRightFront;


    }

    // 接触判定
    private void OnCollisionEnter2D(Collision2D other) {
        foreach(string tag in this.colliderTags){
            if (other.gameObject.CompareTag(tag))  {

            }
        }
    }

    public int ShovelAttack(){
        return 0;
    }

    public int BulldozerAttack(){
        return 0;
    }

    public int TrencherAttack(){
        return 0;
    }    

    public void setPlayer(GameObject obj){
        this.playerObj = obj;
    }

    public void setTags(string[] tags){
        this.colliderTags = tags;
    }

    public void setIsRightFront(bool enable){
        this.isRightFront = enable;
    }

    public void setAttachMentPos(Vector2 pos){
        this.attachMentPos = pos;
    }

}
