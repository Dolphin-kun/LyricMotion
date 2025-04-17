# 「リリックモーションしながら登場退場」プラグイン

## 概要
リリックモーション(と呼ばれる動きの１つ)をしながらテキストを指定した方向から登場退場させることができます。 

## 使い方
1. テキストアイテムを追加して、`登場退場`カテゴリーにある「リリックモーションしながら登場退場」を選択して追加してください。
2. テキストの`文字ごとに分割`をオンにしてください。

## パラメータ
![image](https://github.com/user-attachments/assets/ef5a957a-32a5-4ea6-b05a-bd3e7aa28dcf)

## 効果
### 登場時・退場時
登場時、退場時にエフェクトを適用するか指定します。

### 効果時間
エフェクトの効果時間を指定します。
基本機能と同じです。

### 距離
テキストが表示される距離を指定します。

### 種類・加減速
アニメーションのイージングを指定します。

---

### X方向・Y方向
テキストが表示される方向を指定します。

### 双方向
X,Y方向のプラスマイナス双方向から表示するかを指定します。

### 同一方向
> [!Warning]
> わかりにくい名前になっています。
> 気を付けてお使いください。

退場時に、登場時とは反対の方向にアニメーションをするかどうか指定します。

### 表示方向
- ランダム
- 交互

#### ランダム
方向をランダムに指定します。
「シード」を変えることでランダムな方向を変化することができます。

#### 交互
決まった順番で方向が指定されます。
オフセットを変えることで方向の順番を移動させることができます。

## 注
製作者がC#初心者のためよくわからんコードで書いているかもしれません。
参考にはならないと思います。🙇‍♂️


## 更新履歴
2025/4/17 v1.0
    公開
