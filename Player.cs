using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour {

    private Attachment attachment;

    enum AttachmentType {
        Bulldozer,
        Shovel,
        Trencher,        
    } 

    private Rigidbody2D rb;         // 

    private bool isConnectGamePad = false;  // ゲームパッドが接続されているか
    private bool isJump = false;            // ジャンプ中か
    private int attachmentFlg = 0;          // アタッチメントの状態フラグ
    private AttachmentType equipmentAttachment; // 現在装備しているアタッチメント
    private float preJumpTime = 0;          // ジャンプした瞬間の時間
    private float nowJumpTime = 0;          // 現在の時間
    private Vector2 attachmentMovePosition; // アタッチメントの移動量

    // 以下デバッガ、プランナ設定用フィールド
    public Vector2 position;                // プレイヤー座標情報
    [SerializeField, TooltipAttribute("アタッチメントの相対位置")] private Vector2 attachmentPosition;           // プレイヤーとアタッチメントの相対位置
    [SerializeField, TooltipAttribute("アタッチメント用のオブジェクト")] private GameObject attachmentObj;        // アタッチメント用のオブジェクト
    [SerializeField, TooltipAttribute("最大HP")] public int MaxHP = 10;                  // 最大HP
    [SerializeField, TooltipAttribute("現在HP")] public int CurrntHP = 10;               // 現在HP
    [SerializeField, Range(1.0f, 100.0f), TooltipAttribute("移動量")] private float moveSpeed = 1.0f;           // 移動量
    [SerializeField, Range(1.0f, 100.0f), TooltipAttribute("ジャンプの強さ")] private float jumpPowor = 5.0f;    // ジャンプの強さ
    [SerializeField, TooltipAttribute("AddForceで移動及びジャンプするか")] private bool isAddForce = false;       // AddForceで移動及びジャンプするか
    [SerializeField, TooltipAttribute("空中にいる場合重力加速度を有効にするか")] private bool isGravity = true;    // 重力加速度を有効にするか
    [SerializeField, Range(1, 10), TooltipAttribute("トレンチャ装備時の移動速度低下の割合(大きいほど遅くなる)")] private float TrencherSpeedDown = 2.0f;  // トレンチャ装備時の移動速度低下の割合(大きいほど遅くなる)
    [SerializeField, Range(1, 10000), TooltipAttribute("ジャンプキーをms単位でどのくらい押下できるか(値が大きいほど長くジャンプできる)")] private int jumpPushTime = 100;              

    // 敵接触判定用タグ
    public string[] colliderTags = {};
    // 地面とみなすタグ
    public string[] groundTags = {};

    // 以下処理

    // Start is called before the first frame update
    void Start() {
        // 初期座標の設定
        this.gameObject.transform.position = this.position;

        // RigidbodyComponentの取得
        this.rb = this.GetComponent<Rigidbody2D>();
        if (rb == null) {
            Debug.LogError("Rigidbody2Dが設定されていません");
            return;
        }

        // アタッチメントオブジェクト
        if(this.attachmentObj == null){
            this.attachmentObj = GameObject.Find("Attachment");
            if(this.attachmentObj == null){
                Debug.LogError("アタッチメントオブジェクトが設定されていません");
                return;
            }
        }

        // アタッチメント用クラスの取得
        this.attachment = this.attachmentObj.GetComponent<Attachment>();
        if(this.attachment == null){
            Debug.LogError("アタッチメントのスクリプが設定されていません");            
            return;
        }

        // ゲームパッドが接続されているか
        this.isConnectGamePad = (Gamepad.current == null) ? false : true ;
    }

    // Update is called once per frame
    void Update() {
        // 入力に応じた行動
        this.controller();

        // 座標情報の更新
        this.position = this.gameObject.transform.position;
        this.attachment.attachMentPos =  this.position + this.attachmentPosition + this.attachmentMovePosition;
        //  this.isJump = false;  // デバッグ用

        // ジャンプ中処理
        if(this.isJump) {

        }

        // HP判定
        if (this.CurrntHP <= 0) {

        }

    }

    // 入力に応じた行動処理
    private void controller(){
        // ゲームパッドが接続されているか
        this.isConnectGamePad = (Gamepad.current == null) ? false : true;

        // 更新処理
        if(this.isConnectGamePad){
            this.gamePadControl();
        }else{
            this.keybordControl();
        }
    }

    private void gamePadControl(){
        var gamePad = Gamepad.current;

        // プレイヤー移動量の取得と反映
        Vector2 addSpeed = gamePad.leftStick.ReadValue() * this.moveSpeed;
        Vector2 speed = (this.attachmentFlg == (1 << (int)AttachmentType.Trencher)) ? addSpeed / this.TrencherSpeedDown : addSpeed;

        // 空中での動作
        if(this.isJump){
            if(this.isGravity){
                this.rb.AddForce(speed);                
            }else{
                this.rb.velocity = speed;                
            }
        }else{
            if(this.isAddForce){
                this.rb.AddForce(speed);
            } else {
                this.rb.velocity = speed;            
            }
        }

        // アタッチメント移動量の取得
        this.attachmentMovePosition = gamePad.rightStick.ReadValue();

        // 各ボタン押下毎の処理
        // 上ボタン
        if(gamePad.buttonNorth.isPressed){

        } 
        // 右ボタン
        if(gamePad.buttonEast.isPressed){
            this.jump();
        }
        // 下ボタン
        if(gamePad.buttonSouth.isPressed){
            this.jump();
        } 
        // 左ボタン
        if(gamePad.buttonWest.isPressed){

        }

        // アタッチメントの切り替え
        if(gamePad.dpad.up.isPressed){
            this.equipmentAttachment = AttachmentType.Shovel;
            this.attachmentFlg = (1 << (int)AttachmentType.Shovel);
        } else if(gamePad.dpad.right.isPressed){
            this.equipmentAttachment = AttachmentType.Bulldozer;
            this.attachmentFlg = (1 << (int)AttachmentType.Bulldozer);
        } else if(gamePad.dpad.down.isPressed){
            this.equipmentAttachment = AttachmentType.Trencher;
            this.attachmentFlg = (1 << (int)AttachmentType.Trencher);
        } else if(gamePad.dpad.left.isPressed){
            this.equipmentAttachment = AttachmentType.Bulldozer;
            this.attachmentFlg = (1 << (int)AttachmentType.Bulldozer);            
        }

        // 攻撃処理
        this.attack();
    }

    private void keybordControl(){
        // プレイヤー移動量の取得と反映
        Vector2 addSpeed = Vector2.zero;
        if(Keyboard.current.leftArrowKey.isPressed){
            addSpeed += Vector2.left;
        }
        if(Keyboard.current.rightArrowKey.isPressed){
            addSpeed += Vector2.right;
        }
        if(Keyboard.current.upArrowKey.isPressed){
            addSpeed += Vector2.up;
        }
        if(Keyboard.current.downArrowKey.isPressed){
            addSpeed += Vector2.left;
        }

        addSpeed *= this.moveSpeed;
        Vector2 speed = (this.attachmentFlg == (1 << (int)AttachmentType.Trencher)) ? addSpeed / this.TrencherSpeedDown : addSpeed;

        // アタッチメント移動量の取得
        // プレイヤー移動量の取得と反映
        Vector2 addAttachPos = Vector2.zero;
        if(Keyboard.current.wKey.isPressed){
            addAttachPos += Vector2.left;
        }
        this.attachmentMovePosition = addAttachPos;

        // 空中での動作
        if(this.isJump){
            if(this.isGravity){
                this.rb.AddForce(speed);                
            }else{
                this.rb.velocity = speed;                
            }
        }else{
            if(this.isAddForce){
                this.rb.AddForce(speed);
            } else {
                this.rb.velocity = speed;            
            }
        }


        // ジャンプする場合
        if (Keyboard.current.spaceKey.isPressed) {
            this.jump();
        }

        // 攻撃処理
        this.attack();
    }

    // ジャンプ処理
    private void jump(){
        this.nowJumpTime = Time.time;
        float diff = this.nowJumpTime - this.preJumpTime;

        // ジャンプ可能な最大時間より大きいかつ空中の場合はそのままreturn
        float pushTime = (float)this.jumpPushTime / 1000.0f;
        if(diff > pushTime){
            if(this.isJump){
                return;
            }
        }

        if(this.isAddForce){
            this.rb.AddForce(transform.up * jumpPowor);
        } else {
            this.rb.velocity = transform.up * jumpPowor;            
        }

        if(!this.isJump){
            this.isJump = true;
            this.preJumpTime = Time.time;
        }
    }

    // 攻撃処理
    private int attack() {
        switch(this.equipmentAttachment){
            case AttachmentType.Shovel:
                return this.attachment.ShovelAttack();
            case AttachmentType.Bulldozer:
                return this.attachment.BulldozerAttack();
            case AttachmentType.Trencher:
                return this.attachment.TrencherAttack();
            default:
        return 0;
        }
    }

    // 接触判定
    private void OnCollisionEnter2D(Collision2D other) {
        foreach(string tag in this.groundTags){
            if (other.gameObject.CompareTag(tag))  {
                this.isJump = false;
            }
        }
        foreach(string tag in this.colliderTags){
            if (other.gameObject.CompareTag(tag))  {
                
            }
        }
    }


}
