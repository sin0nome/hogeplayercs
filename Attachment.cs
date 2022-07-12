using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attachment : MonoBehaviour {

    [SerializeField, TooltipAttribute("アタッチメントの座標")] public Vector2 attachMentPos;   // アタッチメント用座標情報(プレイヤーとの相対座標)   
    // 敵接触判定用タグ
    [SerializeField, TooltipAttribute("攻撃可能な敵のタグ")] public string[] colliderTags = {};

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    // 接触判定
    private void OnCollisionEnter2D(Collision2D other) {
        foreach(string tag in this.colliderTags){
            if (other.gameObject.CompareTag(tag))  {
                
            }
        }
    }

    int ShovelAttack(){
        return 0;
    }

    int BulldozerAttack(){
        return 0;
    }
    
    int TrencherAttack(){
        return 0;
    }    

}