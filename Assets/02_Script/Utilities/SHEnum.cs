// 국가
public enum eNationType
{
    KOREA,
}

// 서비스 모드
public enum eServiceMode
{
    None,
    Live,
    Review,
    QA,
    DevQA,
    Dev,
}

// 서비스 상태
public enum eServiceState
{
    None,
    Run,
    Check,
    ConnectMarket,
}

// Enum : 번들 패킹 타입
public enum eBundlePackType
{
    None,   // 아무것도 안함
    All,    // 전체 번들 리패킹
    Update, // 변경된 리소스가 포함되는 번들만 패킹
}

// 씬 종류
public enum eSceneType
{
    None,
    Intro,
    Patch,
    Login,
    Loading,
}

// 데이터 종류
public enum eDataType
{
    None,
    LocalTable,
    ServerData,
    Resources,
    Scene,
    BundleData,
}

// 테이블 포맷타입
public enum eTableType
{
    None,
    SQLite,
    Json,
    XML,
    Byte,
}

// 리소스 데이터 종류
public enum eResourceType
{
    None,
    Prefab,
    Animation,
    Texture,
    Sound,
    Material,
    Text,
}

// Bool
public enum eBOOL
{
    None,
    False,
    True,
}

// 순서
public enum eOrderNum
{
    None,
    First,
    Second,
    Third,
    Fourth,
    Fifth,
    Sixth,
    Seventh,
    Eighth,
    Ninth,
    Tenth,
}