import { NaheulbookPage } from './app.po';

describe('naheulbook App', function() {
  let page: NaheulbookPage;

  beforeEach(() => {
    page = new NaheulbookPage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
