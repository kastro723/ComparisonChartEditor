# ComparisonChartEditor

    [패치 노트]
    
            v1.2.0 - 24.04.24
                   - Scriptable Objetct를 통한 차트 Preview 기능 추가
                   - CSV 파일로 Export가 가능하도록 기능 추가
                   - UI 수정
                   
            v1.1.0 - 24.04.20
                   - 차트 기능 및 UI 버그 수정
                   
            
            v1.0.0 - 24.04.19
                   - ComparisonChartEditor 기능 구현

-------------------------------------------------------------------------------------
![image](https://github.com/kastro723/ComparisonChartEditor/assets/55536937/759d391e-a9df-44a9-b8a3-6889f0c2777b)![image](https://github.com/kastro723/ComparisonChartEditor/assets/55536937/132240be-7e63-4ab9-b743-bd20b603ba9f)



    [기능설명]
    
            스크립터블 오브젝트를 통한  Parameter(a,b,c,d)를 추가해서 공식을 통한 Chart를 볼 수 있다.
            여러 개의 차트를 비교해볼 수 있고, 원하는 값의 차트를 CSV 파일로 Export 할 수 있는 기능을 지원한다.

            옵션 설정
            Bubble(Bg)의 확장 유형(MoveType)을 지원하여, Text의 크기 및 길이에 따른 Bubble의 확장 방향 설정
            Bubble(Bg) 내에서 Text Box의 정렬 유형(TextSort)을 설정
                (Left, Center, Right 방향으로 Bg 내에서 Text Box의 위치를 조정 가능)


    [옵션설명]
    
            Bg Fitter - Bg Content Size Fitter
            Text - UI Text(TextMeshPro)
            Bg - Bubble Sprite
            Picker - Picker Sprite
            Picker Offset Y - Bg와 Picker가 겹치는 간격
            Bg Width - Image의 가로 길이
            Text Width - Text box의 가로 길이
            Text Font Size - Text의 폰트 크기
            Horizontal Padding - Text의 좌, 우 padding
            Vertical Padding - Text의 상, 하 padding
            Paragraph Spacing - Text의 엔터 간 간격
            Line Spacing - Text의 줄 간 간격
            Use If Empty Text Default Size - Text가 없을 시 Image의 크기 (가로(x),  세로(y))
            MoveType - Text의 크기 및 길이에 따른 Bg의 확장 방향 (Up, Down) 
            TextSort - Text의 정렬 방향 (Left, Center, Right)
            Fit - Fit 동작(메서드) 실행 
                (Picker에 따른 Bg와 Text 위치 갱신, 설정된 수치로 Image 및 Text 수정 및 갱신)
            
