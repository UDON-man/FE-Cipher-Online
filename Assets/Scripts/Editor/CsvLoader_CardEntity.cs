using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

[CustomEditor(typeof(LoadCSV_CardEntity))]
public class CsvLoader_CardEntity : Editor
{

	public override void OnInspectorGUI()
	{
		var LoadCSV = target as LoadCSV_CardEntity;
		DrawDefaultInspector();

		if (GUILayout.Button("カードデータの作成"))
		{
			SetCsvDataToScriptableObject(LoadCSV);
		}
	}

	void SetCsvDataToScriptableObject(LoadCSV_CardEntity loadCSV)
	{
		// ボタンを押されたらパース実行
		if (loadCSV.csvFile == null)
		{
			Debug.LogWarning(loadCSV.name + " : 読み込むCSVファイルがセットされていません。");
			return;
		}

		// csvファイルをstring形式に変換
		string csvText = loadCSV.csvFile.text;

		// 改行ごとにパース
		string[] afterParse = csvText.Split('\n');

		//カード画像一覧を取得
		List<Sprite> SpriteList = GetAsset.LoadAll<Sprite>("Assets/Image/Card");

		//英語カード画像一覧を取得
		List<Sprite> EnglishSpriteList = GetAsset.LoadAll<Sprite>("Assets/Image/Card_English");

		// ヘッダー行を除いてインポート
		for (int i = 1; i < afterParse.Length; i++)
		{
			string[] parseByComma = afterParse[i].Split(',');

			int column = 0;

			// 先頭の列が空であればその行は読み込まない
			if (string.IsNullOrEmpty(parseByComma[column]))
			{
				continue;
			}

			// CardEntityのインスタンスをメモリ上に作成
			CEntity_Base cardEntity = CreateInstance<CEntity_Base>();

			//カード名を取得
			string cardName = parseByComma[column];
			cardEntity.CardName = cardName;
			column++;

			//カード画像名
			string spriteName = parseByComma[column];
			column++;

			//カード画像をアタッチ
			foreach (Sprite sp in SpriteList)
			{
				if (sp.name == spriteName)
				{
					cardEntity.CardImage = sp;
					break;
				}
			}

			if (cardEntity.CardImage == null)
			{
				Debug.Log($"{cardName}の画像がありません");
				continue;
			}

			//英語カード画像名
			string englishSpriteName = parseByComma[column];
			column++;

			

			//英語カード画像をアタッチ
			foreach (Sprite sp in EnglishSpriteList)
			{
				if (sp.name == englishSpriteName)
				{
					cardEntity.CardImage_English = sp;
					break;
				}
			}

			if (cardEntity.CardImage_English == null)
			{
				Debug.Log($"{cardName}の英語版画像がありません");
				//continue;
			}

			//ファイル名
			string fileName = cardName; ;

			if(!string.IsNullOrEmpty(parseByComma[column]))
			{
				fileName += parseByComma[column];
			}

			fileName += ".asset";
			column++;

			//ユニット名
			cardEntity.UnitName = parseByComma[column];
			column++;

			//ユニット名(英語)
			cardEntity.UnitName_English = parseByComma[column];
			column++;

			//登場コスト
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.PlayCost = int.Parse(parseByComma[column]);
			}
			column++;

			//CCあるか
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.HasCC = IsTrue(parseByComma[column]);
			}
			column++;

			//CCコスト
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.CCCost = int.Parse(parseByComma[column]);
			}
			column++;

			//カード色①
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.cardColors.Add(DictionaryUtility.GetColor(parseByComma[column], DataBase.CardColorNameDictionary));
			}
			column++;

			//カード色②
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.cardColors.Add(DictionaryUtility.GetColor(parseByComma[column], DataBase.CardColorNameDictionary));
			}
			column++;

			//戦闘力
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.Power = int.Parse(parseByComma[column]);
			}
			column++;

			//支援力
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.SupportPower = int.Parse(parseByComma[column]);
			}
			column++;

			cardEntity.Ranges = new List<int>();
			//射程①
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.Ranges.Add(int.Parse(parseByComma[column]));
			}
			column++;

			//射程②
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.Ranges.Add(int.Parse(parseByComma[column]));
			}
			column++;

			//射程③
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.Ranges.Add(int.Parse(parseByComma[column]));
			}
			column++;
			if(cardEntity.Ranges.Count == 0)
			{
				cardEntity.Ranges.Add(0);
			}

			//性別
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.sex = IsMale(parseByComma[column]);
			}
			column++;

			//武器①
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.Weapons.Add(DictionaryUtility.GetWeapon(parseByComma[column], DataBase.WeaponNameDictionary));
			}
			column++;

			//武器②
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.Weapons.Add(DictionaryUtility.GetWeapon(parseByComma[column], DataBase.WeaponNameDictionary));
			}
			column++;

			//武器③
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.Weapons.Add(DictionaryUtility.GetWeapon(parseByComma[column], DataBase.WeaponNameDictionary));
			}
			column++;

			//カード効果のクラス名
			cardEntity.ClassName = parseByComma[column];
			column++;

			//スキル名①
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.SkillNames.Add(parseByComma[column]);
			}
			column++;

			//スキル名②
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.SkillNames.Add(parseByComma[column]);
			}
			column++;

			//スキル名③
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.SkillNames.Add(parseByComma[column]);
			}
			column++;

			//スキル名④
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.SkillNames.Add(parseByComma[column]);
			}
			column++;

			//支援スキル名①
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.SupportSkillNames.Add(parseByComma[column]);
			}
			column++;

			//支援スキル名②
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.SupportSkillNames.Add(parseByComma[column]);
			}
			column++;

			//コードが出来てるか
			bool isOK = false;
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				isOK= IsTrue(parseByComma[column]);
			}

			if(!isOK)
			{
				continue;
			}

			column++;

			//カードID
			cardEntity.CardID = int.Parse(parseByComma[column]);
			column++;

			//デッキに入れられる最大枚数
			if (!string.IsNullOrEmpty(parseByComma[column]))
			{
				cardEntity.MaxCountInDeck = int.Parse(parseByComma[column]);
			}
			column++;

			string PackName = parseByComma[column];
			column++;

			#region 保存先・ファイル名
			//ファイル名
			fileName = $"{cardEntity.UnitName}_{fileName}";
			//保存先のパス
			string path = $"Assets/CardEntity/{DataBase.CardColorNameDictionary[cardEntity.cardColors[0]]}/{PackName}/{fileName}";
            #endregion

            // インスタンス化したものをアセットとして保存
            var asset = (CEntity_Base)AssetDatabase.LoadAssetAtPath(path, typeof(CEntity_Base));
			if (asset == null)
			{
				// 指定のパスにファイルが存在しない場合は新規作成
				AssetDatabase.CreateAsset(cardEntity, path);
			}
			else
			{
				// 指定のパスに既に同名のファイルが存在する場合は更新
				EditorUtility.CopySerialized(cardEntity, asset);
				AssetDatabase.SaveAssets();
			}

			AssetDatabase.Refresh();
		}

		Debug.Log(loadCSV.name + " : カードデータの作成が完了しました。");
	}

	bool IsTrue(string text)
	{
		if(text == "〇")
		{
			return true;
		}

		return false;
	}

	Sex IsMale(string text)
	{
		if(text == "男")
		{
			return Sex.male;
		}

		return Sex.female;
	}
}

