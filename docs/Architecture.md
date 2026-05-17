# SerialAssistant.Win Architecture

## Overview

SerialAssistant.Win follows a layered architecture pattern with clear separation of concerns.

## Layers

### 1. SerialAssistant.App (Presentation Layer)

**Responsibilities:**
- WPF UI implementation
- Data binding
- User interaction handling
- Window management

**Dependencies:**
- SerialAssistant.Core
- SerialAssistant.Infrastructure

### 2. SerialAssistant.Core (Domain Layer)

**Responsibilities:**
- Domain models and entities
- Business logic interfaces
- Value objects
- Domain events

**Dependencies:**
- None (pure .NET)

**Constraints:**
- No WPF references
- No System.Windows references
- No System.IO.Ports references

### 3. SerialAssistant.Infrastructure (Infrastructure Layer)

**Responsibilities:**
- Serial port service implementation
- File system operations
- External API integrations
- Data persistence

**Dependencies:**
- SerialAssistant.Core

### 4. SerialAssistant.Tests (Test Layer)

**Responsibilities:**
- Unit tests
- Integration tests
- Test fixtures and helpers

**Dependencies:**
- SerialAssistant.Core
- SerialAssistant.Infrastructure
- xUnit

## Project References

```
┌─────────────────────────────────────┐
│      SerialAssistant.App            │
│      (WPF Application)              │
└──────────────┬──────────────────────┘
               │
       ┌───────┴───────┐
       │               │
       ▼               ▼
┌──────────────┐ ┌─────────────────────┐
│   Serial     │ │     Serial          │
│ Assistant.   │ │  Assistant.         │
│ Core         │ │  Infrastructure     │
│ (Domain)     │ │  (Infrastructure)   │
└──────────────┘ └──────────┬──────────┘
                            │
                            ▼
                   ┌─────────────────┐
                   │   Serial        │
                   │ Assistant.      │
                   │ Core            │
                   └─────────────────┘

┌─────────────────────────────────────┐
│      SerialAssistant.Tests          │
│      (Test Project)                 │
└──────────────┬──────────────────────┘
               │
       ┌───────┴───────┐
       │               │
       ▼               ▼
┌──────────────┐ ┌─────────────────────┐
│   Serial     │ │     Serial          │
│ Assistant.   │ │  Assistant.         │
│ Core         │ │  Infrastructure     │
└──────────────┘ └─────────────────────┘
```

## Current Status

Phase 0: Project skeleton initialized. No business logic implemented yet.
