public struct DamageData
{
    public int damage;
    public BaseEntity attacker;
    
    public DamageData(int damage, BaseEntity attacker)
    {
        this.damage = damage;
        this.attacker = attacker;
    }

}