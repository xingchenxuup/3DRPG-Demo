﻿using ARPGDemo.Skill;
using UnityEngine;

namespace ARPGDemo.Character
{
    /// <summary>
    /// 角色输入控制器
    /// </summary>
    public class CharacterInputController : MonoBehaviour
    {
        public BaseBtu joystick;
        private CharacterMotor chMotor;
        private Animator anim;
        private PlayerStatus status;
        public BaseBtu[] skillButtons;
        private CharacterSkillSystem skillSystem;

        private void Awake()
        {
            //查找组件
            // joystick = FindObjectOfType<ETCJoystick>();
            chMotor = GetComponent<CharacterMotor>();
            anim = GetComponentInChildren<Animator>(); //获取player下子组件
            status = GetComponent<PlayerStatus>(); //获取类
            // skillButtons = FindObjectsOfType<ETCButton>();//获取按键组
            skillSystem = GetComponent<CharacterSkillSystem>();
            //GetComponentInChildren
            //GetComponentInParent
        }

        private void OnEnable()
        {
            // //注册事件
            // joystick.onMove.AddListener(OnJoystickMove);
            // joystick.onMoveStart.AddListener(OnJoystickMoveStart);
            // joystick.onMoveEnd.AddListener(OnJoystickMoveEnd);
            //
            for (int i = 0; i < skillButtons.Length; i++)
            {
                if (skillButtons[i].name == "UI_Attack")
                {
                    skillButtons[i].pointDownEvent.AddListener(OnSkillButtonPressed);
                }
                else
                {
                    skillButtons[i].pointDownEvent.AddListener(OnSkillButtonDown);
                    skillButtons[i].pointUpEvent.AddListener(OnSkillButtonUp);
                    skillButtons[i].drawEvent.AddListener(OnSkillButtonDrag);
                }
            }
        }

        private float lastPressTime = -1;

        //当按住普攻键时执行
        private void OnSkillButtonPressed(string name)
        {
            //需求：按住间隔如果过小（2） 则取消攻击
            //间隔小于5秒视于连击

            //间隔：当前按下时间 - 最后按下时间
            float interval = Time.time - lastPressTime;
            if (interval < 1.5) return;
            bool isBatter = interval <= 3;
            // if(interval<=5)
            //{
            //    isBatter = true;
            //}
            // else
            //{
            //    isBatter = false;
            //}
            skillSystem.AttackUseSkill(1001, isBatter);

            lastPressTime = Time.time;
        }

        private void OnSkillButtonDown(string name)
        {
            int id = 0;
            switch (name)
            {
                //case "BaseButton":
                //    id = 1001;
                //    break;
                case "Skill1":
                    id = 1002;
                    break;
                case "Skill2":
                    id = 1003;
                    break;
            }
            //print(name + "+" + "打死你" + "+" + id);
            //CharacterSkillManager skillManager = GetComponent<CharacterSkillManager>();
            ////准备技能（判断条件）
            //SkillData data = skillManager.PrepareSkill(id);//id
            ////Debug.Log("判断条件"+data.skillID);
            ////GetComponent<Skill.CharacterSkillManager>().PrepareSkill(1002);
            //if (data != null)//生成条件
            //{
            //    //Debug.Log("生成条件" + data.prefabName);
            //    skillManager.GenerateSkill(data);

            //}
            skillSystem.AttackUseSkill(id);
            if (select != null && skillSystem.CurrentSkillData != null &&
                skillSystem.CurrentSkillData.skillSelect.Length > 0)
            {
                @select.SetActive(true);
            }
        }

        GameObject select = null;

        private void OnSkillButtonDrag(string name)
        {
            var skill = skillSystem.CurrentSkillData;
            if (select == null)
            {
                select = skillSystem.SkillManager.skillSelcet[name];
                @select.SetActive(true);
            }
            else if (skill != null)
            {
                var SkillDir = skillButtons[1].Dir.x * Camera.main.transform.right +
                               skillButtons[1].Dir.y * Camera.main.transform.forward;

                if (SkillDir == Vector3.zero)
                {
                    SkillDir = transform.forward;
                }
                else
                {
                    SkillDir.y = 0;
                }

                select.transform.forward = SkillDir;
                skillSystem.CurrentSkillData.skillPos =
                    select.GetComponent<Transform>().GetChild(0).GetChild(0).position;
            }
        }

        private void OnSkillButtonUp(string name)
        {
            anim.SetTrigger("Skill1");
            GameObject select = skillSystem.SkillManager.skillSelcet[name];
            select.SetActive(false);
        }

        private void OnJoystickMoveStart()
        {
            //GetComponent<Animator>().SetBool("run", true);
            //GetComponent<PlayerStatus>().chParams.run;
            anim.SetBool(status.chParams.run, true);
        }

        private void OnJoystickMoveEnd()
        {
            //GetComponent<PlayerStatus>().chParams.run;
            //GetComponent<Animator>().SetBool("run", true);
            anim.SetBool(status.chParams.run, false);
        }

        private void OnJoystickMove(Vector2 dir)
        {
            //调用马达移动功能
            //dir.x     左右    0      //dir.y     上下
            //x                 y               z
            chMotor.Movement(new Vector3(dir.x, 0, dir.y));
        }

        private void OnDisable()
        {
            //注销事件
            // joystick.onMove.RemoveListener(OnJoystickMove);
            // joystick.onMoveEnd.RemoveListener(OnJoystickMoveStart);
            // joystick.onMoveEnd.RemoveListener(OnJoystickMoveEnd);
            for (int i = 0; i < skillButtons.Length; i++)
            {
                if (skillButtons[i] == null) continue;
                if (skillButtons[i].name == "BaseButton")
                    skillButtons[i].pointDownEvent.RemoveListener(OnSkillButtonPressed);
                else
                    skillButtons[i].pointUpEvent.RemoveListener(OnSkillButtonDown);
            }
        }
    }
}