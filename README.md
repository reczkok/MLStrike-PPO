# MLStrike-PPO

**FPS w wirtualnej rzeczywistości przy wykorzystaniu ML Agents**

## Autorzy
- Konrad Reczko
- Jakub Truszkowski  
- Jakub Banach
- Eryk Mikołajek

## Opis projektu

MLStrike-PPO to innowacyjne środowisko gry FPS, w którym inteligentni agenci uczą się poruszania i strzelania za pomocą algorytmów uczenia ze wzmocnieniem. Agenci rywalizują ze sobą w trybie self-play, aby stopniowo poprawiać swoje umiejętności. 
W rezultacie projektu gracz może wcielić się w rolę jednego z agentów, używając gogli VR do bezpośredniej rywalizacji z wytrenowanym przeciwnikiem.

### Główne cele
- Stworzenie inteligentnych agentów zdolnych do skutecznej rywalizacji w środowisku FPS
- Implementacja systemu self-play umożliwiającego wzajemne uczenie się agentów
- Integracja z technologią VR dla immersyjnego doświadczenia gracza

## Technologie

### Stack technologiczny
- **Silnik gry:** Unity
- **Uczenie maszynowe:** ML-Agents, MIToolkit, Petting Zoo
- **Wirtualna rzeczywistość:** XR Interaction Toolkit
- **Algorytm:** Proximal Policy Optimization (PPO)

### Architektura agenta
Agent wykorzystuje system percepcji oparty na Ray-Casting w połączeniu z siecią neuronową do podejmowania decyzji w czasie rzeczywistym. 
Szczegółowa struktura została zaprojektowana tak, aby umożliwić złożone zachowania taktyczne przy zachowaniu wydajności obliczeniowej.

## System uczenia

### Obserwacje agenta
Agent otrzymuje informacje o środowisku poprzez:
- Dane o własnej pozycji i orientacji
- Sieć promieni Ray-Cast wykrywających przeszkody i przeciwników
- Dynamiczne sensory o zmiennym kącie nachylenia
- Opcjonalnie: informacje o ostatniej pozycji zauważonego przeciwnika

### Dostępne akcje
- Poruszanie się w 3D (przód/tył/boki)
- Skakanie i kontrola wysokości
- Strzelanie

### System nagród i kar
- **Pozytywne wzmocnienie:** Za wykrycie i trafienie przeciwnika
- **Kary za bierność:** Motywacja do ciągłej eksploracji
- **Główna nagroda:** Za eliminację przeciwnika

## Wyniki i osiągnięcia

### Udane zachowania agentów
- Skuteczna nawigacja i eksploracja środowiska
- Wykorzystywanie elementów terenu dla przewagi taktycznej
- Rozwój podstawowych strategii walki poprzez self-play
- Aktywne poszukiwanie i eliminacja przeciwników

### Ograniczenia
- Czasami chaotyczne ruchy i nieprzewidywalne zachowania
- Niewystarczająca skuteczność przeciwko ludzkim graczom
- Duże wymagania obliczeniowe (wiele godzin treningu na konsumenckiej karcie graficznej)

## Wnioski

Projekt udowodnił skuteczność technik uczenia ze wzmocnieniem w środowisku FPS i możliwość ich integracji z technologią VR. 
Self-play okazał się kluczowy dla rozwoju umiejętności agentów, tworząc znacznie bardziej interesujące rozgrywki niż przeciwko tradycyjnym, oskryptowanym botom.
