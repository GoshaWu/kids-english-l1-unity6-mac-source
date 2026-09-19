Drop LevelContent and Unit1Catalog ScriptableObjects here (Create → Kids English).

L1–L3 scripts are content-frozen. Two-week acceptance is **L1_HelloFriends only**.
L2_FriendMeYou and L3_HappySad are empty stubs (see unit1.levels.json). Do not implement them this week.

HARD LOCK — HelloFriends is L1 v1 only. Do not mount me/you/happy/sad/name.

L1_HelloFriends (HelloFriends scene) uses locked keys only:
  Demo_Hi = Audio/hi (assess=false)
  Demo_Come = Audio/come_play (assess=false)
  three-choice = Bunny / Tree / Rock (Bunny art fallback Art/hi)
  invite = ball → Fox (Fox art fallback Art/come)
  Friend first-look = Art/friend (not a Hi choice)
  Bye required = clipId bye_required_bunny then bye_required_fox, audio Audio/bye, assess=false

Unit vocab grids live in UnitVocab/ and are NOT mounted in this scene.
