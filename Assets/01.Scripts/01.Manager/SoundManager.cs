using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public enum eSoundName
{
    Game_map_music,
    Game_over,
    Button,
    
    Drop,

    // 아이템 사용
    Stripes_bonus,
    Explosion,
    Color_bomb,

    Wrong_match,

    No_match,    
    Warning_time,
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Inst;
    #region ## Data

    [Serializable]
    class SoundData
    {
        public AudioClip AudioClip = null;
        public float Volume = 1f;
        public bool IsLoop = false;
    }

    class PlayingAudio
    {
        public eSoundName Name;
        public AudioSource Source;
    }
    #endregion

    readonly Dictionary<eSoundName, SoundData> _soundDatas = new Dictionary<eSoundName, SoundData>();
    readonly ObjectPoolQueue<AudioSource> _sourcePool = new ObjectPoolQueue<AudioSource>();
    readonly LinkedList<PlayingAudio> _playingList = new LinkedList<PlayingAudio>();
    readonly LinkedList<PlayingAudio> _bgmList = new LinkedList<PlayingAudio>();

    readonly float _fadeDuration = 1f;
    bool _isPause;

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        Init();
    }

    void Init()
    {
        var datas = Util.CSVReadFromResourcesFolder("DataTable/SoundTable");
        for (int i = 0; i < datas.Count; i++)
        {
            SoundData sd = new SoundData();
            eSoundName name = (eSoundName)Enum.Parse(typeof(eSoundName), (string)datas[i]["name"]);
            sd.AudioClip = Resources.Load<AudioClip>($"Sound/{name}");
            sd.Volume = Util.FloatParse($"{datas[i]["volume"]}");
            sd.IsLoop = bool.Parse((string)datas[i]["loop"]);
            _soundDatas.Add(name, sd);
        }

        //Pool Setting
        for (int i = 0; i < 30; i++)
        {
            _sourcePool.Enqueue(CreateAudioSource());
        }

        StartCoroutine(UpdatePlayingList());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        _sourcePool.DestroyAll();
        _playingList.Clear();
        _soundDatas.Clear();
    }

    AudioSource CreateAudioSource()
    {
        GameObject go = new GameObject();
        go.transform.SetParent(this.transform);
        go.name = "AudioSource";
        return go.AddComponent<AudioSource>();
    }

    //사운드재생이 끝난 소스를 풀에 다시집어넣어준다.
    IEnumerator UpdatePlayingList()
    {
        WaitUntil wait = new WaitUntil(() => !_isPause && _playingList.Count > 0);

        while (true)
        {
            yield return wait;

            var node = _playingList.First;
            while (node != null)
            {
                var source = node.Value.Source;
                //재생이 끝났으면
                if (!source.isPlaying)
                {
                    source.Stop();
                    source.clip = null;
                    _sourcePool.Enqueue(source);
                    _playingList.Remove(node);
                }

                node = node.Next;
            }
        }
    }

    public void PlaySoundBGM(eSoundName _name, bool _isFade = true)
    {
        //if (Data.Inst.IsBgmMute)
        //    return;

        var data = _soundDatas[_name];
        var source = _sourcePool.GetObject() ?? CreateAudioSource();

        source.clip = data.AudioClip;
        source.spatialBlend = 0;
        source.loop = data.IsLoop;
        source.volume = data.Volume;
        source.Play();

        if (_isFade)
            source.DOFade(source.volume, _fadeDuration);

        _bgmList.AddLast(new PlayingAudio { Name = _name, Source = source });
    }

    public void PlaySound2D(eSoundName _name, bool _isFade = false)
    {
        //if (Data.Inst.IsSfxMute)
        //    return;

        var data = _soundDatas[_name];
        var source = _sourcePool.GetObject() ?? CreateAudioSource();

        source.clip = data.AudioClip;
        source.spatialBlend = 0;
        source.loop = data.IsLoop;
        source.volume = data.Volume;
        source.Play();

        if (_isFade)
            source.DOFade(source.volume, _fadeDuration);

        _playingList.AddLast(new PlayingAudio { Name = _name, Source = source });
    }

    public void RePlaySoundBGM()
    {
        //if (Data.Inst.IsBgmMute)
        //    return;

        eSoundName _name = eSoundName.Game_map_music;

        var data = _soundDatas[_name];
        var source = _sourcePool.GetObject() ?? CreateAudioSource();

        source.clip = data.AudioClip;
        source.spatialBlend = 0;
        source.loop = data.IsLoop;
        source.volume = data.Volume;
        source.Play();

        source.DOFade(source.volume, _fadeDuration);

        _bgmList.AddLast(new PlayingAudio { Name = _name, Source = source });
    }

    public void StopSoundFX(eSoundName _name, bool _isFade = false)
    {
        var node = _playingList.First;
        while (node != null)
        {
            var source = node.Value.Source;
            if (node.Value.Name.Equals(_name))
            {
                _playingList.Remove(node);
                if (_isFade)
                {
                    source.DOFade(0f, _fadeDuration).OnComplete(() =>
                    {
                        source.Stop();
                        source.clip = null;
                        source.transform.localPosition = Vector3.zero;
                        _sourcePool.Enqueue(source);
                    });
                }
                else
                {
                    source.Stop();
                    source.clip = null;
                    source.transform.localPosition = Vector3.zero;
                    _sourcePool.Enqueue(source);
                }
            }

            node = node.Next;
        }
    }

    public void StopSoundBGM(eSoundName _name, bool _isFade = true)
    {
        var node = _bgmList.First;
        while (node != null)
        {
            var source = node.Value.Source;
            if (node.Value.Name.Equals(_name))
            {
                _bgmList.Remove(node);
                if (_isFade)
                {
                    source.DOFade(0f, _fadeDuration).OnComplete(() =>
                    {
                        source.Stop();
                        source.clip = null;
                        source.transform.localPosition = Vector3.zero;
                        _sourcePool.Enqueue(source);

                    });
                }
                else
                {
                    source.Stop();
                    source.clip = null;
                    source.transform.localPosition = Vector3.zero;
                    _sourcePool.Enqueue(source);
                }
            }

            node = node.Next;
        }
    }

    public void StopFXSoundAll()
    {
        var node = _playingList.First;
        while (node != null)
        {
            var source = node.Value.Source;
            source.Stop();
            source.clip = null;
            _sourcePool.Enqueue(source);

            node = node.Next;
        }
        _playingList.Clear();
    }

    public void StopBGMSoundAll()
    {
        var node = _bgmList.First;
        while (node != null)
        {
            var source = node.Value.Source;
            source.Stop();
            source.clip = null;
            _sourcePool.Enqueue(source);

            node = node.Next;
        }
        _bgmList.Clear();
    }

    public void PauseSound()
    {
        if (_isPause)
            return;

        //loop인 애들만 일시정지. 나머지는 Remove
        var node = _playingList.First;
        while (node != null)
        {
            var source = node.Value.Source;
            if (source.loop)
            {
                source.Pause();
            }
            else
            {
                source.Stop();
                source.clip = null;
                _sourcePool.Enqueue(source);
                _playingList.Remove(node);
            }

            node = node.Next;
        }
        _isPause = true;
    }

    public void ResumeSound()
    {
        if (!_isPause)
            return;

        var node = _playingList.First;
        while (node != null)
        {
            var source = node.Value.Source;
            if (source.loop)
            {
                source.Play();
            }

            node = node.Next;
        }

        _isPause = false;
    }
}