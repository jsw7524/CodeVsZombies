# A Game tree + KMeans approach for Code vs Zombies
[Code vs Zombies Challenge](https://www.codingame.com/ide/puzzle/code-vs-zombies)

## How It Works
The main logic of the game revolves around Ash's strategy to save the most humans by killing zombies. This is achieved by:

1. Using k-means clustering to determine potential target positions for Ash.
2. Evaluating each potential move using a game tree to simulate the outcome of the move and choosing the one that results in the best score.

When all humans are saved or killed, the game ends, and the final score is calculated based on the number of saved humans and killed zombies.


# 使用遊戲樹 + KMeans 方法的《代碼對抗殭屍》策略
[Code vs Zombies Challenge](https://www.codingame.com/ide/puzzle/code-vs-zombies)

## 它是如何工作的
遊戲的主要邏輯圍繞著Ash的策略，透過殺死殭屍來拯救盡可能多的人類。 這是透過以下方式實現的：

1. 使用k-means聚類來確定Ash的潛在目標位置。
2. 使用遊戲樹評估每一個潛在的移動，模擬該移動的結果，並選擇導致最佳分數的那個。

當所有人類都被拯救或被殺死時，遊戲結束，並根據被拯救的人類和被殺死的殭屍的數量計算最終得分。
