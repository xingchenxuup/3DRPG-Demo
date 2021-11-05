using System.Collections;
using UnityEngine;
using Common;
using System;
using System.Collections.Generic;
using ARPGDemo.Character;


namespace ARPGDemo.Skill
{
	/// <summary>
	/// 技能管理器
	/// </summary>
	public class CharacterSkillManager : MonoBehaviour 
	{
        //技能列表
        public SkillData[] skills;
        public Dictionary<string, GameObject> skillSelcet;

        private void Start()
        {
            skillSelcet = new Dictionary<string,GameObject>();
            for (int i = 0; i < skills.Length; i++)
            {
                InitSkill(skills[i]);
                //print(skills[i].name);
            }
        }

        //初始化技能
        private void InitSkill(SkillData data)
        {
            /*
                                           资源映射表
             资源名称                                  资源完整路径
             BaseMeleeAttackSkill=Skills/BaseMeleeAttackSkill 
             */

            //data.prefabName--> data.skillPrefab
            //data.skillPrefab = Resources.Load<GameObject>("Skill/" + data.prefabName);
            //仅仅根据资源名称获取资源
            data.skillPrefab = ResourceManager.Load<GameObject>(data.prefabName);
            //Instantiate(data.skillPrefab);
            Debug.Log("LOAD"+data.skillID);            
            data.owner = gameObject;
            //xcx - 初始化选择器
            if (data.skillSelector != null && data.skillSelector.Length > 0)
            {
                if (transform.Find(data.skillSelector))
                {
                    return;
                }
                var obj = ResourceManager.Load<GameObject>(data.skillSelector);
                Debug.Log(data.skillSelector);
                var instantiate = Instantiate(obj, transform.position, transform.rotation);
                instantiate.transform.parent = transform;
                instantiate.transform.localPosition = Vector3.zero;
                instantiate.transform.localRotation = Quaternion.identity;
                instantiate.transform.localScale = Vector3.one;
                instantiate.SetActive(false);
                skillSelcet.Add(data.name,instantiate);
            }
        }


        //解释 SkillData data = skills.Find(s => s.skillID == id);
        //private SkillData Find(int id,Func<SkillData,bool>handler)
        //{
        //    for(int i=0;i<skills.Length;i++)
        //    {
        //        //if (skills[i].skillID == id)
        //        if (handler(skills[i]))
        //            return skills[i];
        //            return skills[i];
        //    }
        //    return null;   
        //}


        //准备技能（判断技能释放条件：冷却   法力）    
        public SkillData PrepareSkill(int id)
        {
            //根据id  查找技能数据
            SkillData data = skills.Find(s => s.skillID == id);
            //SkillData data = Find(s => s.skillID == id);
            //获取当前角色法力
            float sp = GetComponent<CharacterStatus>().SP;
            //判断条件  返回技能数据
            if (data != null && data.coolRemain <= 0 && data.costSP <= sp)
                return data;
            else
                return null;
        }

        private SkillData Find(Func<SkillData, bool> handler)
        {
            for (int i = 0; i < skills.Length; i++)
            {
                //if (skills[i].skillID == id)  
                if (handler(skills[i]))
                    return skills[i];
            }
            return null;
        }

        //生成技能

        public void GenerateSkill(SkillData data)
        {

            //Debug.Log("GO" + data.skillID);
            //data.skillPrefab = Resources.Load<GameObject>("Skill/" + data.prefabName);
            //Debug.Log("GO" + data.skillPrefab.name);


            //创建技能预制件
            //GameObject skillGo = Instantiate(data.skillPrefab, transform.position, transform.rotation);
            Debug.Log(data.durationTime);
            GameObject skillGo = GameObjectPool.Instance.CreateObject(data.prefabName,data.skillPrefab, data.skillPos, transform.rotation);   
            //xcx - 重置技能释放位置
            data.skillPos = Vector3.zero;
            SkillDeployer deployer = skillGo.GetComponent<SkillDeployer>();
            Debug.Log(deployer.name);
            ////传递技能数据
            deployer.SkillData = data;//内部创建算法对象
            deployer.DeploySkill();//内部执行算法对象

            //销毁技能
            //Destroy(skillGo, data.durationTime);
           
            GameObjectPool.Instance.CollectObject(skillGo, data.durationTime);

            //开启技能冷却
            StartCoroutine(CoolTimeDown(data));
        }

        //技能冷却
        private IEnumerator CoolTimeDown(SkillData data)
        {
            data.coolRemain = data.coolTime;
            while (data.coolRemain > 0)
            {
                yield return new WaitForSeconds(1);
                data.coolRemain--;
            }
        }
    }
}