# **T1 og DS1: Testdesign for API testmiljø**

## **1. Domæne**

### **1.1. Testobjekt**
Primært endpoint:

| Endpoint | Formål | Relevante trusler |
|---|---|---|
| `POST /measurements` | Modtager målepayload med AR hits | T1, DS1 |

Primær entity:

`MeasurementPayload { hits }`

`ARHit { x, y, z, hitType, timestamp }`

### **1.2. Regler for `MeasurementPayload`**

| Felt | Regel |
|---|---|
| `hits` | Skal være en liste med mindst ét AR hit |
| `hits.length` | Må ikke overstige `MAX_HITS` |
| Payload størrelse | Må ikke overstige `MAX_BYTES` |

### **1.3. Regler for `ARHit`**

| Felt | Regel |
|---|---|
| `x`, `y`, `z` | Skal være numeriske og finite værdier |
| Koordinater | Skal ligge inden for `MIN_COORD` og `MAX_COORD` |
| `hitType` | Skal matche en tilladt type i inputkontrakten |
| `timestamp` | Skal være et gyldigt tidspunkt eller afvises efter valgt kontraktregel |

### **1.4. Trusselskobling**

T1: Ugyldige AR hits manipuleres af brugeren, men accepteres og registreres af Firestore tjenesten, hvilket overfylder Firestore dokumentet/kollektionen med data.

DS1: Den ondsindede aktør sender store mængder falsk data til Firestore, så appen påvirkes af øget ressourceforbrug og svartider, eller overskrider specifikke kvoter som lukker tjenesten midlertidigt.

---

## **2. Ækvivalensklasser**

### **2.1. T1: ugyldige AR hits**

| Klasse | Input | Forventning |
|---|---|---|
| EC-T1 | `hits` mangler | ValueError |
| EC-T2 | `hits` er tom liste | ValueError |
| EC-T3 | `hits` er ikke en liste | ValueError |
| EC-T4 | AR hit hvor `x`, `y` eller `z` mangler | ValueError |
| EC-T5 | AR hit hvor `x`, `y` eller `z` er string, bool eller null | ValueError |
| EC-T6 | AR hit med `NaN`, `Infinity` eller ikke finite værdi | ValueError |
| EC-T7 | AR hit med koordinat uden for tilladt interval | ValueError |
| EC-T8 | AR hit med ukendt `hitType` | ValueError |
| EC-T9 | AR hit med ugyldigt eller manglende `timestamp` | ValueError eller afvisning efter valgt politik |
| EC-T10 | Gyldig målepayload med gyldige AR hits | OK |

### **2.2. DS1: store mængder falsk data**

| Klasse | Input | Forventning |
|---|---|---|
| EC-D1 | Payload under `MAX_BYTES` og under `MAX_HITS` | OK |
| EC-D2 | Payload præcis på `MAX_BYTES` | OK |
| EC-D3 | Payload over `MAX_BYTES` | PayloadTooLarge |
| EC-D4 | Antal AR hits præcis på `MAX_HITS` | OK |
| EC-D5 | Antal AR hits over `MAX_HITS` | PayloadTooLarge |
| EC-D6 | Gentagne payloads under `MAX_BYTES` og under `MAX_HITS` | OK |
| EC-D7 | Gentagne payloads hvor samlet mængde medfører kontrolleret afvisning efter valgt testgrænse | PayloadTooLarge eller ResourceLimitExceeded |

### **2.3. Info**

*Ækvivalensklasserne holdes adskilt efter trussel.  
For T1 testes ugyldige AR hits gennem manglende felter, forkerte datatyper, ikke finite værdier, ugyldige koordinater, ukendt `hitType` og ugyldigt timestamp.  
For DS1 testes store mængder falsk data gennem payloadstørrelse, antal AR hits og gentagne payloads.  
T1 handler om gyldigheden af AR hits. DS1 handler om datamængden, ressourceforbrug, svartider og Firestore kvoter.*

---

## **3. Grænseværditest**

### **3.1. T1: `hits` længde**

Krav: `hits.length >= 1`

| Test | Input | Forventning |
|---|---|---|
| BV-TL1 | `hits.length = 0` | ValueError |
| BV-TL2 | `hits.length = 1` | OK |

### **3.2. T1: koordinatværdi**

Krav: `MIN_COORD <= coordinate <= MAX_COORD`

| Test | Input | Forventning |
|---|---|---|
| BV-TC1 | `coordinate = MIN_COORD - 1` | ValueError |
| BV-TC2 | `coordinate = MIN_COORD` | OK |
| BV-TC3 | `coordinate = 0` | OK |
| BV-TC4 | `coordinate = MAX_COORD` | OK |
| BV-TC5 | `coordinate = MAX_COORD + 1` | ValueError |

### **3.3. DS1: antal AR hits pr. request**

Krav: `hits.length <= MAX_HITS`

| Test | Input | Forventning |
|---|---|---|
| BV-DH1 | `hits.length = MAX_HITS - 1` | OK |
| BV-DH2 | `hits.length = MAX_HITS` | OK |
| BV-DH3 | `hits.length = MAX_HITS + 1` | PayloadTooLarge |

### **3.4. DS1: payload størrelse**

Krav: `payloadBytes <= MAX_BYTES`

| Test | Input | Forventning |
|---|---|---|
| BV-DP1 | `payloadBytes = MAX_BYTES - 1` | OK |
| BV-DP2 | `payloadBytes = MAX_BYTES` | OK |
| BV-DP3 | `payloadBytes = MAX_BYTES + 1` | PayloadTooLarge |

### **3.5. DS1: gentagne payloads**

Krav: gentagne payloads må ikke medføre ukontrolleret datamængde mod Firestore

| Test | Input | Forventning |
|---|---|---|
| BV-DR1 | Gentagne payloads under valgt testgrænse | OK |
| BV-DR2 | Gentagne payloads præcis på valgt testgrænse | OK |
| BV-DR3 | Gentagne payloads over valgt testgrænse | PayloadTooLarge eller ResourceLimitExceeded |

---

## **4. Decision Table Tests**

### **4.1. Decision Table A – T1: ugyldige AR hits**

Betingelser:
- A1: `hits` findes og er en liste?
- A2: `hits.length >= 1`?
- A3: alle AR hits har numeriske koordinater?
- A4: alle koordinater er finite?
- A5: alle koordinater ligger inden for tilladt interval?
- A6: alle AR hits har gyldig `hitType`?
- A7: alle AR hits har gyldigt `timestamp`?

Handling:
- H1: Accepter payload
- H2: Afvis med `ValueError`

| Regel | A1 | A2 | A3 | A4 | A5 | A6 | A7 | Handling |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| DT-T1 | T | T | T | T | T | T | T | H1 |
| DT-T2 | F | - | - | - | - | - | - | H2 |
| DT-T3 | T | F | - | - | - | - | - | H2 |
| DT-T4 | T | T | F | - | - | - | - | H2 |
| DT-T5 | T | T | T | F | - | - | - | H2 |
| DT-T6 | T | T | T | T | F | - | - | H2 |
| DT-T7 | T | T | T | T | T | F | - | H2 |
| DT-T8 | T | T | T | T | T | T | F | H2 |

### **4.2. Decision Table B – DS1: store mængder falsk data**

Betingelser:
- B1: `payloadBytes <= MAX_BYTES`?
- B2: `hits.length <= MAX_HITS`?
- B3: gentagne payloads holder sig inden for valgt testgrænse?

Handling:
- K1: Accepter payload
- K2: Afvis med `PayloadTooLarge`
- K3: Afvis med `ResourceLimitExceeded`

| Regel | B1 | B2 | B3 | Handling |
|---|---:|---:|---:|---|
| DT-D1 | T | T | T | K1 |
| DT-D2 | F | - | - | K2 |
| DT-D3 | T | F | - | K2 |
| DT-D4 | T | T | F | K3 |

---

## **5. CRUD(L) testcases**

Entity: `MeasurementRecord { id, hits, createdAt }`

CRUD(L) bruges her til at teste oprettelse, efterfølgende læsning og kontrol af, at afviste payloads ikke lagres i test repository. T1 og DS1 behandles som separate trusler: T1 afvises på grund af ugyldige AR hits, mens DS1 afvises på grund af datamængde.

| ID | Operation | Input | Forventning | Trussel |
|---|---|---|---|---|
| CRUD-T1 | CREATE | Payload hvor `hits` mangler | ValueError, ingen lagring | T1 |
| CRUD-T2 | CREATE | Payload med tom `hits` liste | ValueError, ingen lagring | T1 |
| CRUD-T3 | CREATE | Payload med ugyldig koordinat | ValueError, ingen lagring | T1 |
| CRUD-T4 | CREATE | Payload med ukendt `hitType` | ValueError, ingen lagring | T1 |
| CRUD-T5 | CREATE | Gyldig målepayload med gyldige AR hits | Success | T1 |
| CRUD-D1 | CREATE | Payload med for mange AR hits | PayloadTooLarge, ingen lagring | DS1 |
| CRUD-D2 | CREATE | Payload over `MAX_BYTES` | PayloadTooLarge, ingen lagring | DS1 |
| CRUD-D3 | CREATE | Gentagne payloads over valgt testgrænse | ResourceLimitExceeded, ingen lagring af afvist payload | DS1 |
| CRUD-R1 | READ | Eksisterende måling oprettet fra gyldig payload | Success | T1 |
| CRUD-R2 | READ | Id fra afvist T1 payload | NotFound eller intet id oprettet | T1 |
| CRUD-R3 | READ | Id fra afvist DS1 payload | NotFound eller intet id oprettet | DS1 |
| CRUD-L1 | LIST | Efter gyldig oprettelse | Listen indeholder gyldige records | T1 |
| CRUD-L2 | LIST | Efter afviste T1 payloads | Listen indeholder ikke ugyldige AR hits | T1 |
| CRUD-L3 | LIST | Efter afviste DS1 payloads | Listen indeholder ikke afviste store payloads | DS1 |

---

## **6. Cycle process test**

### **6.1. T1: ugyldige AR hits**

Cycle: Valid create → Read → Invalid create → List

| Step | Operation | Input | Forventning |
|---|---|---|---|
| 1 | `POST /measurements` | Gyldig målepayload med gyldige AR hits | Record oprettes |
| 2 | `GET /measurements/{id}` | Id fra step 1 | Record kan læses |
| 3 | `POST /measurements` | Payload med ugyldigt AR hit | Request afvises |
| 4 | `GET /measurements` | Liste efter step 3 | Afvist payload fremgår ikke |

### **6.2. DS1: store mængder falsk data**

Cycle: Under grænse → På grænse → Over grænse → Kontrolleret respons

| Step | Operation | Input | Forventning |
|---|---|---|---|
| 1 | `POST /measurements` | Payload under `MAX_HITS` og `MAX_BYTES` | Accepteres |
| 2 | `POST /measurements` | Payload præcis på `MAX_HITS` eller `MAX_BYTES` | Accepteres |
| 3 | `POST /measurements` | Payload over `MAX_HITS` eller `MAX_BYTES` | Afvises kontrolleret |
| 4 | `POST /measurements` | Ny payload under grænserne efter afvisning | Systemet svarer fortsat korrekt |

### **6.3. DS1: gentagne payloads**

Cycle: Gentagne payloads under grænse → Gentagne payloads på grænse → Gentagne payloads over grænse

| Step | Operation | Input | Forventning |
|---|---|---|---|
| 1 | `POST /measurements` | Gentagne payloads under valgt testgrænse | Accepteres |
| 2 | `POST /measurements` | Gentagne payloads præcis på valgt testgrænse | Accepteres |
| 3 | `POST /measurements` | Gentagne payloads over valgt testgrænse | Afvises kontrolleret |
| 4 | `GET /measurements` | Liste efter afvisning | Afviste store payloads fremgår ikke |

---

## **7. Testpyramiden**

| Lag | Fokus | Eksempler |
|---|---|---|
| Unit | T1: validering af AR hits | `validateRejectsMissingHits`, `validateRejectsEmptyHits`, `validateRejectsInvalidCoordinate`, `validateRejectsUnknownHitType` |
| Unit | DS1: grænser for datamængde | `validateRejectsTooManyHits`, `validateRejectsPayloadOverMaxBytes` |
| Integration | T1: endpoint, service og test repository | `postMeasurementInvalidARHitReturnsBadRequest`, `rejectedInvalidARHitIsNotPersisted` |
| Integration | DS1: endpoint, service og test repository | `postMeasurementTooLargeReturnsRejected`, `rejectedLargePayloadIsNotPersisted` |
| System/E2E | Fuldt Unity til API til Firestore flow | Uden for den praktiske implementation |

---
