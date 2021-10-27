# MCex
Multicast Searcher

ただマルチキャスト探査するだけ。  
自己満仕様  
.  
.  
マルチキャスト探査する側(マルチキャストでリクエスト送信＆レスポンス受信)  
List<string> MCex.MCSearch.MulticastSercher(string mc_address ,string yc_address ,int reqport ,int resport ,int timeout)  
    
マルチキャスト探査に反応する側(マルチキャストでリクエスト受信＆レスポンス送信)  
void MCex.MCSearch.MulticastResponser(string mc_address ,string yc_address ,int reqport , int resport)  
  
マルチキャスト探査に反応する側を終了させる  
void MCex.MCSearch.StopResponser
