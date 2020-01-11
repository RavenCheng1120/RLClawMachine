using UnityEngine;
using System.Collections;

public class MovimientosClaw : MonoBehaviour {

    public float alturaMaxima; //最大高度
	public float alturaMinima; //最低高度
	public float correctorSeparacionTubos;

	public GameObject Tubos; //管子
	public GameObject Motor; //馬達
	public GameObject Gancho; //鉤子
	public Animator animatorClaw;
    
    public float speed;
    public float speed_Claw;
    Rigidbody rigidBody;

    public Transform AlturaGancho; //吊鉤高度

	// Estos son simples cubos que usa para comparar los limites de movimiento
	// 這些是用於比較運動極限的簡單多維數據集

	// 限制器：前後左右，來保證夾子不會跑出機台外
	public Transform limitadorLeft;
    public Transform limitadorRight;
    public Transform limitadorFront;
    public Transform limitadorBack;


    public bool PuedeControlarse; //可以控制
    bool SoltarPremio; //獎品掉落

    public bool[] LlegadoALaCesta; //到達籃子

	bool dentroDeLaCesda; //內籃
	bool bajarGanchoYSoltarPremio; //下掛鉤
	bool subirGanchoDeLaCesta; //上去
    bool goToCenter;


    // Use this for initialization
    void Start () {
        animatorClaw = Gancho.gameObject.GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        PuedeControlarse = true;
        SoltarPremio = false;
	}


    // Update is called once per frame
    void Update() {

    }


	void FixedUpdate () {

		//Fijar el motor al movimiento en X/Z del gancho
		//將馬達固定在掛鉤的X/Z方向上
		Motor.transform.position= new Vector3(transform.position.x,Motor.transform.position.y,transform.position.z);
		Tubos.transform.position= new Vector3(Tubos.transform.position.x,Tubos.transform.position.y,Motor.transform.position.z+correctorSeparacionTubos);

        //soltar el premio
		// 爪子上升、到洞口上方
        if (SoltarPremio)
        {
            if (AlturaGancho.position.y <=alturaMaxima)
            {
                transform.Translate(0, speed_Claw * Time.deltaTime, 0);
            }
            else
            {
                LlegadoALaCesta[0] = true;
            }

            if (transform.position.x >= limitadorLeft.transform.position.x+0.5f)
            {      
                    transform.Translate(speed * -1 * Time.deltaTime, 0, 0);
            }
            else
            {
                LlegadoALaCesta[1] = true;
            }


            if (transform.position.z >= limitadorFront.transform.position.z+0.5f)
            {
                    transform.Translate(0, 0, speed * -1 * Time.deltaTime);
            }
            else
            {
                LlegadoALaCesta[2] = true;
            }

			//comprobar que el gancho esta dentro de la cesta
			// 檢查鉤子在籃子裡
			if (LlegadoALaCesta[0]&& LlegadoALaCesta[1]&& LlegadoALaCesta[2])
            {
                //Debug.Log("Prize is inside the basket");
                dentroDeLaCesda = true;

				//Iniciar corroutina para bajar el gancho y soltar el premio
				//啟動corroutina降低掛鉤並釋放獎品
				if (dentroDeLaCesda) {
                    StartCoroutine(SoltarPremioEnLaCesta(1.5f));
                    dentroDeLaCesda = false;
                }
            }

        }


        if (bajarGanchoYSoltarPremio)
        {
            if (AlturaGancho.position.y > alturaMinima)
            {
                //Debug.Log("Put down claw and prize");
                transform.Translate(0, speed_Claw * -1 * Time.deltaTime, 0);
            }
            else {
                StartCoroutine(AbrirClawEnlaCesta(1.0f));
                bajarGanchoYSoltarPremio = false;
            }
        }


        if (subirGanchoDeLaCesta)
        {
            if (AlturaGancho.position.y <= alturaMaxima)
            {
                transform.Translate(0, speed_Claw * Time.deltaTime, 0);
            }
            else
            {
                StartCoroutine(backToCenter(0.7f));
            }
        }


        if(goToCenter)
        {
            if (transform.position.x <= limitadorLeft.transform.position.x + 5.2f)
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
            }
            else
            {
                LlegadoALaCesta[0] = true;
            }


            if (transform.position.z <= limitadorFront.transform.position.z + 4.5f)
            {
                transform.Translate(0, 0, speed * Time.deltaTime);
            }
            else
            {
                LlegadoALaCesta[1] = true;
            }

            if(LlegadoALaCesta[0] && LlegadoALaCesta[1])
            {
                goToCenter = false;
                PuedeControlarse = true;
                LlegadoALaCesta[0] = false;
                LlegadoALaCesta[1] = false;
            }
        }

		// 可以控制爪子
        if (PuedeControlarse)
        {
            //Movimentos Laterales
            if (transform.position.x > limitadorLeft.transform.position.x) // 在左邊邊界之內
            {
                if (Input.GetKey(KeyCode.A)) // 按下A鍵
                {
                    transform.Translate(speed * -1 * Time.deltaTime, 0, 0); //sphere往左移動
                }
            }

            if (transform.position.x < limitadorRight.transform.position.x)
            {
                if (Input.GetKey(KeyCode.D))
                { 
                    transform.Translate(speed * Time.deltaTime, 0, 0);
                }
            }

            //Movimientos Forward
            if (transform.position.z < limitadorBack.transform.position.z)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    transform.Translate(0, 0, speed * Time.deltaTime);  
                }
            }

            if (transform.position.z > limitadorFront.transform.position.z)
            {
                if (Input.GetKey(KeyCode.S))
                {
                    transform.Translate(0, 0, speed * -1 * Time.deltaTime);   
                }
            }

            //Bajar Gancho
            // 下鉤
            if (AlturaGancho.position.y > alturaMinima)
            {
                if (Input.GetKey(KeyCode.Space))
                {                  
                    transform.Translate(0, speed_Claw * -1 * Time.deltaTime, 0);
                    //Debug.Log("下降："+ (AlturaGancho.position.y - alturaMinima).ToString());
                }
            }

            else
            {
                StartCoroutine(CerrarClaw(2.0f));
                PuedeControlarse = false;
            }

        }
    }

    //coroutina para recoger el premio
	//獎品收集 
    IEnumerator CerrarClaw(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        CerrarClaw();

        yield return new WaitForSeconds(waitTime);

        //Debug.Log("Subiendo gancho");
        SoltarPremio = true;

    }

	//coroutina para soltar el premio
	//協程發布獎品，將獎品放入籃
	IEnumerator SoltarPremioEnLaCesta (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        bajarGanchoYSoltarPremio = true;
        SoltarPremio = false;
        //Debug.Log("Lower the claw");

        yield return new WaitForSeconds(waitTime);

    }

	//coroutina para soltar el premio
	//協程發布獎品，籃子裡的開爪
	IEnumerator AbrirClawEnlaCesta (float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        AbrirClaw();
        yield return new WaitForSeconds(waitTime);
        subirGanchoDeLaCesta = true;
        LlegadoALaCesta[0] = false;
        LlegadoALaCesta[1] = false;
        LlegadoALaCesta[2] = false;

    }

    //放完獎品後，會回到機台中央
    IEnumerator backToCenter(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        goToCenter = true;
        subirGanchoDeLaCesta = false;

        yield return new WaitForSeconds(waitTime);

    }

    // 開啟爪子
    public void AbrirClaw() {
        //Debug.Log("Open claw");
        animatorClaw.SetBool("Abrir", true);
        animatorClaw.SetBool("Cerrar", false);

    }

	// 關閉爪子
    public void CerrarClaw()
    {
        //Debug.Log("Close claw");
        animatorClaw.SetBool("Abrir", false);
        animatorClaw.SetBool("Cerrar", true);

    }

    public void SoltarPremioTrue()
    {
        SoltarPremio = true;
    }

}
