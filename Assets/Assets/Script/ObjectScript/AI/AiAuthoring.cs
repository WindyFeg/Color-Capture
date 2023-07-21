using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace CoCa.Ai
{

    public class AiAuthoring : MonoBehaviour
    {
        public int scoreWall;
        public int scoreInCentre;
        public int scoreHaftChooseMap;
        public int negScoreAroundEnemy;

        public int stuckSpot;


    }

    public partial struct Ai : IComponentData
    {
        public int _scoreWall;
        public int _scoreInCentre;
        public int _scoreHaftChooseMap;
        public int _negScoreAroundEnemy;
        public int _stuckSpot;
    }

    public class AiBaker : Baker<AiAuthoring>
    {
        public override void Bake(AiAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Ai
            {
                _scoreWall = authoring.scoreWall,
                _scoreInCentre = authoring.scoreInCentre,
                _scoreHaftChooseMap = authoring.scoreHaftChooseMap,
                _negScoreAroundEnemy = authoring.negScoreAroundEnemy,
                _stuckSpot = authoring.stuckSpot
            });
        }
    }
}
