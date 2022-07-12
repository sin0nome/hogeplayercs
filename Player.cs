using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]

public class Player : MonoBehaviour {

    enum Attachment {
        Bulldozer,
        Shovel,
        Trencher,        
    } 

    private Rigidbody2D rb;         // 
    public Vector2 attachMentPos;   // アッタッチメント用座標情報

    private bool isConnectGamePad = false;  // ゲームパッドが接続されているか
    private bool isJump = false;            // ジャンプ中か
    private int attachmentFlg = 0;          // アタッチメントの状態フラグ
    private Attachment equipmentAttachment; // 現在装備しているアタッチメント


    // 以下デバッガ、プランナ設定用フィールド
    public Vector2 position;                // プレイヤー座標情報
    public int MaxHP = 10;                  // 最大HP
    public int CurrntHP = 10;               // 現在HP
    public float moveSpeed = 1.0f;          // 移動量
    public float jumpPowor = 1.0f;          // ジャンプの強さ
    public bool isAddForce = false;         // AddForceで移動及びジャンプするか
    public bool isGravity = true;           // 重力加速度を有効にするか
    public float TrencherSpeedDown = 2.0f;  // トレンチャ装備時の移動速度低下の割合(大きいほど遅くなる)

    public KeyCode[] key;                   // キー

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

        // ゲームパッドが接続されているか
        this.isConnectGamePad = (Gamepad.current == null) ? false : true ;
    }

    // Update is called once per frame
    void Update() {
        // 座標情報の更新
        this.position = this.gameObject.transform.position;

        // 入力に応じた行動
        this.controller();

        this.isJump = false;  // デバッグ用

        // ジャンプ中処理
        if(this.isJump) {

        }

        // HP判定
        if (this.CurrntHP <= 0) {

        }

    }

    // 入力に応じた行動処理
    void controller(){
        // ゲームパッドが接続されているか
        this.isConnectGamePad = (Gamepad.current == null) ? false : true;

        // 更新処理
        if(this.isConnectGamePad){
            this.gamePadControl();
        }else{
            this.keybordControl();
        }
    }

    void gamePadControl(){
        var gamePad = Gamepad.current;

        // プレイヤー移動量の取得と反映
        Vector2 addSpeed = gamePad.leftStick.ReadValue() * this.moveSpeed;
        Vector2 speed = (this.attachmentFlg == (1 << (int)Attachment.Trencher)) ? addSpeed / this.TrencherSpeedDown : addSpeed;

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
        this.attachMentPos = gamePad.rightStick.ReadValue();

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
            this.equipmentAttachment = Attachment.Shovel;
            this.attachmentFlg = (1 << (int)Attachment.Shovel);
        } else if(gamePad.dpad.right.isPressed){
            this.equipmentAttachment = Attachment.Bulldozer;
            this.attachmentFlg = (1 << (int)Attachment.Bulldozer);
        } else if(gamePad.dpad.down.isPressed){
            this.equipmentAttachment = Attachment.Trencher;
            this.attachmentFlg = (1 << (int)Attachment.Trencher);
        } else if(gamePad.dpad.left.isPressed){
            this.equipmentAttachment = Attachment.Bulldozer;
            this.attachmentFlg = (1 << (int)Attachment.Bulldozer);            
        }

        // 攻撃処理
        this.attack();
    }

    void keybordControl(){
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
        Vector2 speed = (this.attachmentFlg == (1 << (int)Attachment.Trencher)) ? addSpeed / this.TrencherSpeedDown : addSpeed;

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
    void jump(){
        if(this.isJump){
            return;
        }

        if(this.isAddForce){
            this.rb.AddForce(transform.up * jumpPowor);
        } else {
            this.rb.velocity = transform.up * jumpPowor;            
        }
        this.isJump = true;        
    }

    // 攻撃処理
    int attack() {
        switch(this.equipmentAttachment){
            case Attachment.Shovel:

            case Attachment.Bulldozer:

            case Attachment.Trencher:

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