# SerialPlotter
The graph drawing software from Serial COM port data  
シリアルCOMポートの数値データをグラフ化するWindowsアプリケーションです。  
標準的なシリアル通信の数値可視化が容易に行えます。
組込機器のデバッグ等に最適です。


<img src="https://github.com/meerstern/SerialPlotter/blob/master/serialplotter.png" width="360">


## 仕様
  * 「,(カンマ)」区切りで数字列をグラフ化します
  * 数字列が4つ以下の場合は数字列分描画します
  * 数字列が4つより多い場合は先頭から4つ分描画します
  * 最小、最大値からグラフが自動でスケーリングします
  * 整数だけでなく、小数も描画可能です
  * 画面下に最新の受信した文字列が表示されます
  * 「Data」で一度に表示するデータ数を選択できます
  * 「Average」で指定した回数の平均化処理した値を表示できます
  * 「Save」にチェックを入れると接続時の時間名でCSVファイルとして保存されます
  * CSVファイルは実行ファイルと同じ場所に保存されます
 
 
License - MIT
