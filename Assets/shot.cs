using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class shot : MonoBehaviour
{
    public static float movementSpeed = 5f;
    public game_character character1;
    public game_character character2;

    private int special_power_count = 0;
    private bool[] buttons_enable = new bool[5];

    void Start()
    {
        for (int i = 0; i < buttons_enable.Length; i++) { buttons_enable[i] = true; }

        character1 = new game_character();
        character2 = new game_character();

        character1.enable_character = true;
        character2.char_pos_x = 4f;

        Create(character1);
        Create(character2);

        single_bullet(character1);
        single_bullet(character2);
    }
    
    public void Create(game_character character)
    {
        character.bullets.Add(new Stack<GameObject>());
        character.bullets.Add(new Stack<GameObject>());
        character.bullets.Add(new Stack<GameObject>());

        character.bullet = GameObject.Find("bullet");
        character.character_obj = (GameObject)Instantiate(GameObject.Find("character"));//second character
        character.character_obj.transform.position = new Vector3(
            character.character_obj.transform.position.x-character.char_pos_x,
            character.character_obj.transform.position.y,
            character.character_obj.transform.position.z
        );
        character.character_obj.SetActive(false); 

        character.main_bullet = (GameObject)Instantiate(GameObject.Find("bullet"));//second character
        character.main_bullet.transform.position = new Vector3(
            character.main_bullet.transform.position.x-character.char_pos_x,
            character.main_bullet.transform.position.y,
            character.main_bullet.transform.position.z
        );
        character.main_bullet.SetActive(false); 
        character.main_bullet.name = "bullet";

        for (int i = 0; i < 10; i++)
        {
            GameObject obj = (GameObject)Instantiate(character.bullet);
            obj.SetActive(false); 
            character.bullets[0].Push(obj);
        }
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = (GameObject)Instantiate(character.bullet);
            obj.SetActive(false); 
            character.bullets[1].Push(obj);
        }
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = (GameObject)Instantiate(character.bullet);
            obj.SetActive(false); 
            character.bullets[2].Push(obj);
        }
    }
    void start_coroutine(Vector3 obj_pos, Vector3 obj_rot, int active_bullets_index,game_character character)
    {
        StartCoroutine( object_pooling_start(obj_pos,obj_rot,active_bullets_index,character));
    }
    IEnumerator object_pooling_start(Vector3 obj_pos, Vector3 obj_rot, int active_bullets_index,game_character character)
    {
        yield return new WaitUntil(() => character.enable_character == true);
        StartCoroutine( this.yolla(obj_pos, obj_rot, active_bullets_index, character) );
        yield return new WaitForSeconds( 6f );
        StartCoroutine( this.topla( active_bullets_index, character) );
    }
    IEnumerator yolla(Vector3 obj_pos, Vector3 obj_rot, int active_bullets_index,game_character character)
    {
        while( true )
        { 
            yield return new WaitForSeconds( character.next_bullet_wait );

            GameObject obj = get_bullet(active_bullets_index, character);
            obj.transform.position = obj_pos;
            obj.transform.eulerAngles = obj_rot;

            character.active_bullets[active_bullets_index].Add(obj);
            if(character.send_double_bullet)
            {
                yield return new WaitForSeconds( 0.2f );

                GameObject obj2 = get_bullet(active_bullets_index, character);
                obj2.transform.position = obj_pos;
                obj2.transform.eulerAngles = obj_rot;

                character.active_bullets[active_bullets_index].Add(obj2);
            }
        }
    }
    IEnumerator topla(int active_bullets_index,game_character character)
    {
        while( true )
        {
            yield return new WaitForSeconds( character.next_bullet_wait );

            GameObject obj = character.active_bullets[active_bullets_index][0];
            character.active_bullets[active_bullets_index].RemoveAt(0);
            push_bullet(obj,active_bullets_index, character);
            if(character.send_double_bullet)
            {
                yield return new WaitForSeconds( 0.2f );

                GameObject obj2 = character.active_bullets[active_bullets_index][0];
                character.active_bullets[active_bullets_index].RemoveAt(0);
                push_bullet(obj2, active_bullets_index, character);
            }
        }
    }


    GameObject get_bullet(int active_bullets_index, game_character character)
    {
        if( character.bullets[active_bullets_index].Count > 0 )
        {
            GameObject obj = character.bullets[active_bullets_index].Pop();
            obj.gameObject.SetActive( true );
            return obj;
        }
 
        return (GameObject)Instantiate(character.bullet);
    }
    void push_bullet(GameObject obj,int active_bullets_index, game_character character)
    {
        obj.gameObject.SetActive(false);
        obj.transform.position = character.bullet.transform.position;
        character.bullets[active_bullets_index].Push(obj);
    }
    
    void single_bullet(game_character character)
    {
        character.active_bullets.Add(new List<GameObject>());
        start_coroutine(new Vector3(
            character.bullet.transform.position.x-character.char_pos_x,
            character.bullet.transform.position.y,
            character.bullet.transform.position.z
        ), character.bullet.transform.eulerAngles, 0,character);
    }
    void double_bullet(game_character character)//ard arda
    {
        character.send_double_bullet = true;
    }
    void cross_bullet(game_character character)
    {
        character.active_bullets.Add(new List<GameObject>());
        character.active_bullets.Add(new List<GameObject>());
        start_coroutine(new Vector3(
            character.bullet.transform.position.x-character.char_pos_x,
            character.bullet.transform.position.y,
            character.bullet.transform.position.z
        ), new Vector3(
            character.bullet.transform.eulerAngles.x,
            character.bullet.transform.eulerAngles.y+45,
            character.bullet.transform.eulerAngles.z
        ), 1,character);

        start_coroutine(new Vector3(
            character.bullet.transform.position.x-character.char_pos_x,
            character.bullet.transform.position.y,
            character.bullet.transform.position.z
        ), new Vector3(
            character.bullet.transform.eulerAngles.x,
            character.bullet.transform.eulerAngles.y-45,
            character.bullet.transform.eulerAngles.z
        ), 2,character);
    }
    void faster_bullet()
    {
        shot.movementSpeed = 10f;//global variable
    }
    void more_consecutive(game_character character)
    {
        character.next_bullet_wait = 1f;
    }
    void OnGUI()
    {
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 20;
        if (GUI.Button(new Rect((Screen.width/2)-60, (Screen.height/2)+200, 120, 50), "double")&&buttons_enable[0])
        {
            double_bullet(character1);
            double_bullet(character2);
            check_special_power_count();
            buttons_enable[0] = false;
        }
        if (GUI.Button(new Rect((Screen.width/2)-60, (Screen.height/2)+250, 120, 50), "cross")&&buttons_enable[1])
        {
            cross_bullet(character1);
            cross_bullet(character2);
            check_special_power_count();
            buttons_enable[1] = false;
        }
        if (GUI.Button(new Rect((Screen.width/2)-60, (Screen.height/2)+300, 120, 50), "faster")&&buttons_enable[2])
        {
            faster_bullet();
            check_special_power_count();
            buttons_enable[2] = false;
        }
        if (GUI.Button(new Rect((Screen.width/2)-60, (Screen.height/2)+350, 120, 50), "consecutive")&&buttons_enable[3])
        {
            more_consecutive(character1);
            more_consecutive(character2);
            check_special_power_count();
            buttons_enable[3] = false;
        }
        if (GUI.Button(new Rect((Screen.width/2)-60, (Screen.height/2)+400, 120, 50), "second character")&&buttons_enable[4])
        {
            character2.enable_character = true;
            character2.character_obj.SetActive(true);
            character2.main_bullet.SetActive(true);
            check_special_power_count();
            buttons_enable[4] = false;
        }
        if (GUI.Button(new Rect(Screen.width-120, 0, 120, 50), "finish game"))
        {
            SceneManager.LoadScene("main");
        }
        GUI.Label(new Rect(10, 10, 300, 50), "Special Power Usage: "+special_power_count);
    }
    void check_special_power_count()
    {
        special_power_count += 1;
        if(special_power_count == 3)
        {
            for (int i = 0; i < buttons_enable.Length; i++) { buttons_enable[i] = false; }
        }
    }
}
public class game_character 
{
    public List<Stack<GameObject>> bullets = new List<Stack<GameObject>>();
    public List<List<GameObject>> active_bullets = new List<List<GameObject>>();
    public GameObject bullet;
    public GameObject character_obj;  
    public GameObject main_bullet;  

    public float next_bullet_wait = 2f;

    public bool send_double_bullet = false;
    public bool enable_character = false;

    public float char_pos_x = 0f;

    // Start is called before the first frame update
    
}