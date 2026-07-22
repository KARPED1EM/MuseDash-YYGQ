# CustomHitEffect

## 功能

Muse Dash 自定义判定特效 Mod。替换游戏内 PERFECT / GREAT / PASS / EARLY / LATE 判定文字为你自己准备的贴图

## 设置

配置文件在 `${Your muse dash folder}/UserData/CustomHitEffect.cfg` 

## 安装

### 再此之前

- 你需要 *MelonLoader 0.6.1* 或者更高的版本, 并在你的Muse Dash上面工作 

### 步骤
- 将 `CustomHitEffect.dll` 放入 `Muse Dash/Mods/`
- 将 `BattleEffects文件夹` 放入 `${Your muse dash folder}/UserData` 里
- 启动一次游戏生成 `CustomHitEffect.cfg`, 将需要的特效包名字写入配置文件

## 特效包目录结构

每个特效包是 `UserData/BattleEffects/` 下的一个子目录，包含四个风格文件夹：

## 如何创建自己的特效包

```
UserData/BattleEffects/
└── 我的特效包/                    
    ├── Default/
    │   ├── ScorePerfect.png
    │   ├── ScoreGreat.png
    │   ├── ScorePass.png
    │   ├── GoldPerfect.png
    │   ├── GoldPerfectBg.png
    │   ├── GoldGreat.png
    │   ├── GoldGreatBg.png
    │   ├── Early.png              
    │   └── Late.png               
    ├── DJMax/                     
    │   ├── ScorePerfect_djmax.png
    │   ├── ScoreGreat_djmax.png
    │   └── ...
    ├── GC/                        
    │   ├── ScorePerfectGC.png
    │   └── ...
    └── Touhou/                    
        ├── ScorePerfect_touhou_black.png
        └── ...
```

> [!NOTE] 
> 名字不要修改，只修改图像即可

## Credits

- **Authors** — KARPED1EM & Doushabo
- **Art** — art assets from Muse Dash Custom Play

## License

[GPL-3.0](LICENSE) © KARPED1EM & Doushabo
