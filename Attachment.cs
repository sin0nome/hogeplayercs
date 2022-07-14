using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachment : MonoBehaviour {

    public enum AttachmentType {
        Bulldozer,
        Shovel,
        Trencher,        
    }

    [SerializeField, TooltipAttribute("プレイヤーオブジェクト")] private GameObject playerObj;        // アタッチメント用のオブジェクト
    [SerializeField, TooltipAttribute("アタッチメントの座標")] public Vector2 attachMentPos;   // アタッチメント用座標情報(プレイヤーとの相対座標)   
    [SerializeField, TooltipAttribute("攻撃可能な敵のタグ")] public AttachmentType type;

    // 敵接触判定用タグ
    private string[] colliderTags;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(this.playerObj == null){
            return;
        }

        // プレイヤーに追従(相対座標で動く)
        this.gameObject.transform.position = this.attachMentPos;
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

}
