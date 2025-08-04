# NinjaTrader Coding Best Practices and Error Reference

_Last updated: 2025-08-04_

This document provides guidance on naming conventions, error troubleshooting, and workflow best practices when developing custom NinjaTrader indicators using C#.

---

## 📘 Best Practices

### Naming Conventions
- Avoid underscores (`_`) in indicator class names and filenames.
- Stick to PascalCase for class names and filenames (e.g., `EMALowMarker.cs`, not `EMA_Low_Marker.cs`).
- Keep names short, clear, and unique within the NinjaScript environment.

### Indicator File Structure
- Always include both:
  - The main indicator class.
  - The `#region NinjaScript generated code` for linking with strategies and MarketAnalyzer.
- Version tag inside the file using a comment such as:
  ```csharp
  // Version: 1.0.0
  ```

### Debugging Tips
- Use `Print()` for runtime debugging.
- Confirm `OnBarUpdate()` is being called.
- Log values of inputs and calculated outputs for verification.

---

## 🛠️ Common Errors and Fixes

| Error Code | Explanation | Fix |
|------------|-------------|-----|
| `CS0103`   | Name does not exist in the current context | Check spelling or variable scope |
| `CS0246`   | Type or namespace not found | Ensure using directives are present |
| `CS0616`   | Attribute usage error | Fix malformed `[Description()]` or `[Browsable()]` attributes |
| `CS0234`   | Type does not exist in the namespace | Likely a typo or missing using directive |
| `CS1503`   | Argument type mismatch | Make sure argument types match function signatures |
| `CS1513`   | Missing closing brace | Review code blocks for unclosed braces |

---

## 🧭 Versioning Strategy

- Each `.cs` file should include a visible version comment.
- Repository versioning should follow semantic versioning:
  - `MAJOR.MINOR.PATCH` (e.g., `1.2.0`)
- Use Git tags or commit messages to track updates.

---

## 📦 Integration and Git Workflow

### Recommended:
1. **Add new indicator via** NinjaTrader: `New > NinjaScript > Indicator`.
2. **Paste full code** into the new file, overwriting template.
3. **Compile** and test in a chart.
4. **In VSCode**, stage, commit, and push changes to GitHub:
   ```bash
   git add .
   git commit -m "Add MACDFull v1.0.0"
   git push
   ```

---

Maintained by: _Darryl Conliffe_

