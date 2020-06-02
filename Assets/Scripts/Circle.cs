using UnityEngine;

//两圆关系
public enum CircleRelations { 
    //外离
    Separate, 
    //内含
    Contain, 
    //相交
    Intersect
}

public struct Circle {
    public Vector2 center;
    public float radius;

    //点在圆中
    public bool IsPointInCircle(Vector2 point) {
        return Vector2.Distance(point, center) <= radius;
    }

    //获取两圆之间的关系
    public CircleRelations GetCircleRelation(Circle otherCircle) {
        //圆心距离
        float distance = Vector2.Distance(otherCircle.center, center);

        if(distance > radius + otherCircle.radius) {
            return CircleRelations.Separate;
        }
        else if(distance < radius - otherCircle.radius) {
            return CircleRelations.Contain;
        }

        return CircleRelations.Intersect;
    }
}