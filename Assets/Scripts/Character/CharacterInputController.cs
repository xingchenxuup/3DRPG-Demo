using ARPGDemo.Skill;
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
        private Camera mainCam;

        private void Awake()
        {
            //查找组件
            // joystick = FindObjectOfType<ETCJoystick>();
            chMotor = GetComponent<CharacterMotor>();
            anim = GetComponentInChildren<Animator>(); //获取player下子组件
            status = GetComponent<PlayerStatus>(); //获取类
            // skillButtons = FindObjectsOfType<ETCButton>();//获取按键组
            skillSystem = GetComponent<CharacterSkillSystem>();
            mainCam = Camera.main;
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
                    if (((SkillBtn) skillButtons[i]).isDrag)
                    {
                        skillButtons[i].pointUpEvent.AddListener(OnSkillButtonUp);
                        skillButtons[i].drawEvent.AddListener(OnSkillButtonDrag);
                    }
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
                case "Skill3":
                    id = 1004;
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

            var btn = GetBtn(name);
            if (!((SkillBtn) btn).isDrag || skillSystem.CurrentSkillData == null)
            {
                return;
            }

            if (selector == null)
            {
                selector = skillSystem.SkillManager.skillSelcet[name];
                selector.SetActive(true);
            }
            selector.SetActive(true);
            if (selector != null && skillSystem.CurrentSkillData != null &&
                skillSystem.CurrentSkillData.skillSelector.Length > 0)
            {
                var selTran = selector.GetComponent<Transform>();
                switch (skillSystem.CurrentSkillData.selectorReleaseType)
                {
                    case SelectorReleaseType.Direction:
                        selTran.GetChild(1).gameObject.SetActive(false);
                        selTran.GetChild(1).GetComponent<RangerSelector>().enabled = true;
                        break;
                    case SelectorReleaseType.Range:
                        selTran.GetChild(0).gameObject.SetActive(false);
                        selTran.GetChild(1).gameObject.SetActive(true);
                        selTran.GetChild(1).position = transform.position;
                        selTran.GetChild(1).GetComponent<RangerSelector>().enabled = false;
                        break;
                }
            }
        }

        GameObject selector = null;

        private void OnSkillButtonDrag(string name)
        {
            var skill = skillSystem.CurrentSkillData;
            if (skill == null)
            {
                return;
            }
            var btn = GetBtn(name);
            if (selector != null && btn != null)
            {
                switch (skill.selectorReleaseType)
                {
                    case SelectorReleaseType.Direction:
                        var SkillDir = btn.Dir.x * mainCam.transform.right +
                                       btn.Dir.y * mainCam.transform.forward;
                        if (SkillDir == Vector3.zero)
                        {
                            SkillDir = transform.forward;
                        }
                        else
                        {
                            SkillDir.y = 0;
                        }
                        selector.transform.forward = SkillDir;
                        skill.skillPos =
                            selector.GetComponent<Transform>().GetChild(1).position;
                        break;
                    case SelectorReleaseType.Range:
                        Vector3 skillPos = transform.position+(btn.Dir.x * mainCam.transform.right +
                                           btn.Dir.y * mainCam.transform.forward)*skill.attackDistance;
                        skillPos.y = 0.1f;
                        var sel = selector.GetComponent<Transform>().GetChild(1);
                        sel.position = skillPos;
                        skill.skillPos =
                            sel.position;
                        break;
                }

                if (skill.isStillLookSkill)
                {
                    transform.LookAt(skill.skillPos);
                }
            }
        }

        private void OnSkillButtonUp(string name)
        {
            if (selector)
            {
                selector.SetActive(false);
                selector = null;
            }
            if (skillSystem.CurrentSkillData == null)
            {
                return;
            }
            anim.SetTrigger("Skill1");
            if (skillSystem.CurrentSkillData.isLookSkill)
            {
                transform.LookAt(skillSystem.CurrentSkillData.skillPos);
            }
        }

        private BaseBtu GetBtn(string name)
        {
            foreach (var skillButton in skillButtons)
            {
                if (skillButton.name == "UI_" + name)
                {
                    return skillButton;
                }
            }

            return null;
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